import { spawn } from "node:child_process";
import { mkdtemp, readdir, readFile, writeFile, rm } from "node:fs/promises";
import { tmpdir } from "node:os";
import { basename, dirname, join, resolve } from "node:path";

// Decode Ogg originals to reproducible 32 kHz mono PCM using Chromium Web Audio.
const [chrome, folder] = process.argv.slice(2);
if (!chrome || !folder) throw new Error("Usage: node decode-foley.mjs <chrome.exe> <foley folder>");
const profile = await mkdtemp(join(tmpdir(), "brotato-foley-"));
const child = spawn(chrome, [
  "--headless=new", "--disable-gpu", "--no-first-run", "--remote-debugging-port=0",
  `--user-data-dir=${profile}`, "about:blank",
], { windowsHide: true, stdio: ["ignore", "ignore", "pipe"] });
let socket;
try {
  const browserUrl = await new Promise((resolveAddress, reject) => {
    const timer = setTimeout(() => reject(new Error("Chromium startup timed out")), 15000);
    child.stderr.on("data", (part) => {
      const address = String(part).match(/ws:\/\/127\.0\.0\.1:\d+\/devtools\/browser\/\S+/);
      if (address) {
        clearTimeout(timer);
        resolveAddress(address[0]);
      }
    });
    child.once("error", reject);
    child.once("exit", (status) => reject(new Error(`Chromium exited with ${status}`)));
  });
  const targetUrl = browserUrl.replace("ws:", "http:").split("/devtools")[0] + "/json/list";
  const [target] = await (await fetch(targetUrl)).json();
  socket = new WebSocket(target.webSocketDebuggerUrl);
  await new Promise((done, fail) => {
    socket.addEventListener("open", done, { once: true });
    socket.addEventListener("error", fail, { once: true });
  });
  let requestId = 0;
  function call(method, params) {
    const id = ++requestId;
    return new Promise((done, fail) => {
      function receive(event) {
        const reply = JSON.parse(event.data);
        if (reply.id !== id) return;
        socket.removeEventListener("message", receive);
        if (reply.error || reply.result.exceptionDetails) fail(new Error(JSON.stringify(reply)));
        else done(reply.result.result.value);
      }
      socket.addEventListener("message", receive);
      socket.send(JSON.stringify({ id, method, params }));
    });
  }
  const files = (await readdir(folder)).filter((name) => name.endsWith(".ogg"));
  for (const file of files) {
    const source = await readFile(join(folder, file));
    const expression = `(async () => {
      const bytes = Uint8Array.from(atob("${source.toString("base64")}"), x => x.charCodeAt(0));
      const context = new OfflineAudioContext(1, 1, 32000);
      const clip = await context.decodeAudioData(bytes.buffer);
      const pcm = new Int16Array(clip.length);
      for (let channel = 0; channel < clip.numberOfChannels; channel++) {
        const samples = clip.getChannelData(channel);
        for (let i = 0; i < samples.length; i++) {
          pcm[i] += Math.round(samples[i] * 32766 / clip.numberOfChannels);
        }
      }
      const b = new Uint8Array(pcm.buffer);
      let data = "";
      for (let i = 0; i < b.length; i += 8192) data += String.fromCharCode(...b.slice(i, i + 8192));
      return btoa(data);
    })()`;
    const decoded = Buffer.from(await call("Runtime.evaluate", { expression, awaitPromise: true, returnByValue: true }), "base64");
    const header = Buffer.alloc(44);
    header.write("RIFF");
    header.writeUInt32LE(decoded.length + 36, 4);
    header.write("WAVEfmt ", 8);
    header.writeUInt32LE(16, 16);
    header.writeUInt16LE(1, 20);
    header.writeUInt16LE(1, 22);
    header.writeUInt32LE(32000, 24);
    header.writeUInt32LE(64000, 28);
    header.writeUInt16LE(2, 32);
    header.writeUInt16LE(16, 34);
    header.write("data", 36);
    header.writeUInt32LE(decoded.length, 40);
    await writeFile(resolve(folder, file.replace(/\.ogg$/, ".wav")), Buffer.concat([header, decoded]));
  }
  console.log(`Decoded ${files.length} source sounds.`);
} finally {
  socket?.close();
  child.kill();
  if (dirname(resolve(profile)) === resolve(tmpdir()) && basename(profile).startsWith("brotato-foley-")) {
    await rm(profile, { recursive: true, force: true, maxRetries: 5, retryDelay: 150 }).catch(() => {});
  }
}
