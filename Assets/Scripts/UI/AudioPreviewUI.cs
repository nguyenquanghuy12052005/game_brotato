using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public sealed partial class AudioPreviewController
{
    private static readonly Color Shell = new(.047f,.078f,.10f);
    private static readonly Color Panel = new(.078f,.127f,.16f);
    private static readonly Color Surface = new(.113f,.177f,.21f);
    private static readonly Color Border = new(.18f,.276f,.32f);
    private static readonly Color Ink = new(.97f,.96f,.90f);
    private static readonly Color Muted = new(.68f,.76f,.78f);
    private static readonly Color Gold = new(1f,.72f,.32f);
    private static readonly Color Mint = new(.49f,.89f,.84f);
    private GUIStyle titleStyle, headingStyle, smallStyle, normalStyle, accentStyle, buttonStyle, pickedStyle, playStyle, rowStyle;
    private Texture2D buttonTexture, buttonOver, pickedTexture, playTexture, playOver, rowTexture;
    private readonly Dictionary<string,Texture2D> rowIcons = new();
    private const int W=1600,H=900;

    private void BuildStyles()
    {
        if(titleStyle!=null) return;
        GUIStyle Label(int size, Color color, bool bold = false, bool wrap = false) => new(GUI.skin.label)
        {
            fontSize = size, fontStyle = bold ? FontStyle.Bold:FontStyle.Normal,
            wordWrap = wrap, richText = false, normal = {textColor = color},
            padding = new RectOffset(0,0,0,0)
        };
        GUIStyle Button(Texture2D baseTexture,Texture2D hover, Color color,int size) => new(GUI.skin.button)
        {
            fontSize=size, fontStyle=FontStyle.Bold, wordWrap=false,
            padding=new RectOffset(9,9,2,2),
            normal={background=baseTexture,textColor=color},
            active={background=hover,textColor=color}, hover={background=hover,textColor=color},
            focused={background=hover,textColor=color}, border=new RectOffset()
        };
        titleStyle=Label(38,Ink,true);
        headingStyle=Label(22,Ink,true,true);
        smallStyle=Label(13,Muted,false,true);
        normalStyle=Label(16,Ink,false,true);
        accentStyle=Label(14,Mint,true,true);
        buttonTexture=Swatch(Surface);
        buttonOver=Swatch(Border);
        pickedTexture=Swatch(new Color(.19f,.33f,.32f));
        playTexture=Swatch(Gold);
        rowTexture=Swatch(new Color(.09f,.14f,.17f));
        buttonStyle=Button(buttonTexture,buttonOver,Ink,15);
        pickedStyle=Button(pickedTexture,buttonOver,Mint,15);
        playOver=Swatch(new Color(1,.87f,.61f));
        playStyle=Button(playTexture,playOver,Shell,17);
        rowStyle=Button(rowTexture,buttonOver,Ink,15);
        rowStyle.alignment=TextAnchor.MiddleLeft;
        rowStyle.wordWrap=true;
    }

    private static Texture2D Swatch(Color color)
    {
        var texture=new Texture2D(1,1) {hideFlags=HideFlags.HideAndDontSave};
        texture.SetPixel(0,0,color);
        texture.Apply();
        return texture;
    }

    private void OnGUI()
    {
        BuildStyles();
        float k=Mathf.Min(Screen.width/(float)W,Screen.height/(float)H);
        if(k<=0) return;
        Fill(new Rect(0,0,Screen.width,Screen.height),Shell);
        Matrix4x4 matrix=GUI.matrix;
        GUI.matrix=Matrix4x4.TRS(new Vector3((Screen.width-W*k)/2,(Screen.height-H*k)/2,0),Quaternion.identity,new Vector3(k,k,1));
        Header();
        if (current!=null)
        {
            DrawCategories();
            DrawList();
            DrawStage();
            DrawInfo();
            DrawControls();
            PaintText(new Rect(24,870,1552,22),"SPACE  Phát / Dừng     ·     ← / →  Biến thể     ·     WAV từ dự án     ·     Studio độc lập",smallStyle);
            Shortcuts();
        }
        else PaintText(new Rect(28,130,900,32),entries==null ? "Đang mở catalog Audio Preview..." : "Đang chuẩn bị sân minh họa...",normalStyle);
        GUI.matrix=matrix;
    }

    private void Header()
    {
        Fill(new Rect(0,0,W,105),Panel);
        Fill(new Rect(24,19,5,67),Gold);
        PaintText(new Rect(43,18,220,22),"BROTATO 2D",accentStyle);
        PaintText(new Rect(43,38,800,49),"Audio Preview Studio",titleStyle);
        PaintText(new Rect(780,52,441,28),"Nghe thử · xem mốc · duyệt biến thể",normalStyle);
        PaintText(new Rect(1310,40,240,24),entries==null ? "HEXCORE" : $"{entries.Length} mục  /  10 danh mục",accentStyle);
    }

    private void DrawCategories()
    {
        Box(new Rect(24,121,206,722));
        PaintText(new Rect(39,135,180,28),"Danh mục",headingStyle);
        for(int i=0; i<Categories.Length; i++)
        {
            Rect line=new(37,174+i*48,179,39);
            if (Click(line,CategoryLabels[i],i==category ? pickedStyle : buttonStyle))
            {
                GUI.FocusControl(null);
                SwitchCategory(i);
            }
        }
        PaintText(new Rect(39,692,173,94),"Chọn nhóm rồi bấm một mục để xem và nghe. Duyệt được lưu theo từng biến thể trên máy.",smallStyle);
    }

    private void DrawList()
    {
        Box(new Rect(244,121,282,722));
        PaintText(new Rect(258,135,256,28),string.IsNullOrWhiteSpace(query) ? CategoryLabels[category] : "Kết quả tìm kiếm",headingStyle);
        PaintText(new Rect(258,172,256,18),"Tìm trong toàn bộ thư viện",smallStyle);
        GUI.SetNextControlName("studio-search");
        string typed=GUI.TextField(new Rect(258,191,254,30),query,100);
        if(typed!=query) {query=typed;listScroll=Vector2.zero;}
        string needle=Fold(query.Trim());
        if(needle.Length==0 && Click(new Rect(258,228,254,29),
            branchFilter.Length==0 ? "Tất cả nhánh   ›" : branchFilter+"   ›"))
        {
            var branches = entries.Where(x=>x.category==Categories[category]).Select(BranchName).Distinct().Prepend("").ToArray();
            branchFilter=branches[(Array.IndexOf(branches,branchFilter)+1)%branches.Length];
            listScroll=Vector2.zero;
        }
        var filtered=Array.FindAll(entries,entry=>needle.Length==0
            ? entry.category==Categories[category] && (branchFilter.Length==0 || BranchName(entry)==branchFilter)
            : Fold(entry.name+" "+entry.branch).Contains(needle));
        var area=new Rect(254,263,264,568);
        listScroll=GUI.BeginScrollView(area,listScroll,new Rect(0,0,247,Mathf.Max(area.height,58*filtered.Length)));
        for(int i=0;i<filtered.Length;i++)
        {
            if (i*58+58 < listScroll.y || i*58 > listScroll.y+area.height) continue;
            var item=filtered[i];
            float y=i*58;
            Rect rect=new(2,y,243,53);
            if (Click(rect,"",item==current ? pickedStyle : rowStyle))
            {
                GUI.FocusControl(null);
                Select(item,true);
            }
            Texture2D icon=RowIcon(item);
            int inset=icon==null ? 12 : 42;
            if(icon!=null) GUI.DrawTexture(new Rect(9,y+12,26,26),icon,ScaleMode.ScaleToFit);
            PaintText(new Rect(inset,y+3,236-inset,30),$"{item.id:000}  {item.name}",normalStyle);
            PaintText(new Rect(inset,y+33,235-inset,18),ReviewStatus(item),smallStyle);
        }
        if(filtered.Length==0) PaintText(new Rect(5,12,224,25),"Không tìm thấy mục.",smallStyle);
        GUI.EndScrollView();
    }

    private void DrawStage()
    {
        Box(new Rect(542,121,704,496));
        PaintText(new Rect(558,133,659,27),"Sân minh họa",headingStyle);
        Rect screen=new(554,170,680,382);
        Fill(screen,new Color(.048f,.082f,.10f));
        if(stage.Background!=null)
        {
            GUI.color=new Color(1,1,1,.64f);
            GUI.DrawTexture(screen,stage.Background,ScaleMode.ScaleAndCrop);
            GUI.color=Color.white;
        }
        else if(stage.SourceImage!=null) GUI.DrawTexture(screen,stage.SourceImage,ScaleMode.ScaleToFit);
        else if(stage.Image!=null) GUI.DrawTexture(screen,stage.Image,ScaleMode.ScaleToFit);
        if(current.category=="Menu và UI") DrawUiSample(screen);
        if(Track || current.category=="Môi trường")
        {
            if(stage.Background==null && Track)
                PaintText(new Rect(screen.x+25,screen.y+140,screen.width-50,48),Variant.name,titleStyle);
            Fill(new Rect(screen.x+17,screen.y+screen.height-57,screen.width-34,40),Panel);
            PaintText(new Rect(screen.x+29,screen.y+screen.height-50,screen.width-55,26),$"{Status}  ·  {Variant.name}",normalStyle);
        }
        float expected=LoopLayer ? Variant.layers[mode-3].seconds : Variant.seconds;
        float length=Mathf.Max(.01f,(sound.Playing || reachedEnd) && sound.Length>0 ? sound.Length : expected);
        float progress=Mathf.Clamp01(sound.Position/length);
        Fill(new Rect(557,565,674,5),Border);
        Fill(new Rect(557,565,674*progress,5),Gold);
        if (AnimatedTake)
            foreach (var layer in Variant.layers ?? Array.Empty<AudioPreviewLayer>())
                foreach (float mark in layer.marks ?? Array.Empty<float>())
                    Fill(new Rect(557+674*Mathf.Clamp01(mark/length),563,1,9),
                        layer.label.StartsWith("Va chạm") ? Gold : Mint);
        PaintText(new Rect(558,579,240,25),$"{Status}   {sound.Position:0.00} / {length:0.00} s",accentStyle);
        PaintText(new Rect(807,579,420,24),ReferenceAudio ? "Bản đối chiếu nghe độc lập" : LoopLayer ? "Loop rời · nghe độc lập" : stage.IsMelee ?
            "Pha: " + stage.Phase : Variant.loop ? "Chế độ nghe lặp" : "Một lượt",smallStyle);
    }

    private Texture2D RowIcon(AudioPreviewEntry item)
    {
        string name = item.id switch
        {
            >=20 and <=48 => "pistol",
            50 or >=56 and <=61 or 76 => "assassin",
            51 or >=62 and <=67 or 74 => "dao",
            52 or >=68 and <=73 or 75 => "qi",
            >=77 and <=107 => "mage",
            >=108 and <=116 => "normal-brown",
            >=117 and <=119 => "skeleton",
            >=120 and <=122 => "golem",
            >=123 and <=125 => "yeti",
            _ when item.category=="Nhân vật" => "player",
            _ => null
        };
        if (name==null) return null;
        if(!rowIcons.TryGetValue(name,out Texture2D art))
        {
            art=Resources.Load<Texture2D>("BrotatoMenu/Portraits/"+name);
            rowIcons.Add(name,art);
        }
        return art;
    }

    private void DrawUiSample(Rect screen)
    {
        var labels=new[] {"Di chuột","Nhấn","Xác nhận"};
        int hovered=-1;
        for(int i=0;i<labels.Length;i++)
        {
            Rect rect=new(screen.x+64+i*185,screen.y+160,172,44);
            if(rect.Contains(Event.current.mousePosition)) hovered=i;
            if(Click(rect,labels[i],i==2 ? playStyle : buttonStyle)) PlayCue();
        }
        if(Event.current.type!=EventType.Layout && hovered!=hoverButton)
        {
            hoverButton=hovered;
            if(hovered==0) PlayCue();
        }
    }

    private void DrawControls()
    {
        Box(new Rect(542,630,704,213));
        Rect control=new(556,642,109,32);
        if(Click(control,"▶ Phát",playStyle)) PlayCue();
        control.x+=115;
        if(Click(control,"■ Dừng")) StopCue();
        control.x+=115;
        if(Click(control,"↻ Phát lại")) PlayCue();
        control.x+=115;
        if(Click(control,"‹ Biến thể")) StepVariant(-1);
        control.x+=115;
        if(Click(control,"Biến thể ›")) StepVariant(1);
        control.x+=115;
        if(Click(control,automatic ? "Tự lặp ✓" : "Tự lặp",automatic ? pickedStyle : buttonStyle)) automatic=!automatic;

        Toggle(new Rect(556,681,108,29),"VFX",ref vfx);
        Toggle(new Rect(671,681,108,29),"Animation",ref animate);
        if(Click(new Rect(786,681,108,29),dummy ? "Dummy ✓":"Dummy",dummy ? pickedStyle : buttonStyle)) SetDummy();
        Toggle(new Rect(901,681,108,29),"Tắt nhạc",ref muteMusic);
        Toggle(new Rect(1016,681,108,29),"Tắt SFX",ref muteSfx);
        if(Click(new Rect(1131,681,101,29),half?"0,5×":"1×",half ? pickedStyle : buttonStyle)) half=!half;
        Level(556,721,207,"Master",ref master,0,1,true);
        Level(790,721,207,"Nhạc",ref music,0,1,true);
        Level(1025,721,207,"SFX",ref sfx,0,1,true);
        Level(556,770,207,"Pitch test (nửa cung)",ref pitch,-5,5,false);
        Level(790,770,207,"Zoom camera",ref zoom,.8f,1.25f,false);
        PaintText(new Rect(1025,775,203,51),"Tốc độ và pitch cùng dùng đồng hồ sample.",smallStyle);
    }

    private void DrawInfo()
    {
        Box(new Rect(1262,121,314,722));
        PaintText(new Rect(1277,135,282,27),"Thông tin cue",headingStyle);
        infoScroll=GUI.BeginScrollView(new Rect(1275,174,290,652),infoScroll,new Rect(0,0,269,1060));
        PaintText(new Rect(0,0,269,50),current.name,headingStyle);
        PaintText(new Rect(0,53,269,22),$"{version+1}/{current.variants.Length}  ·  {Variant.name}",accentStyle);
        PaintText(new Rect(0,83,269,56),current.description,normalStyle);
        int y=149;
        if(Click(new Rect(0,y,84,29),"Bản phối",mode==0 ? pickedStyle:buttonStyle)) SwitchMode(0);
        if(!string.IsNullOrEmpty(Variant.miss) && Click(new Rect(91,y,77,29),"Hụt",mode==1 ? pickedStyle:buttonStyle)) SwitchMode(1);
        if(!string.IsNullOrEmpty(current.old) && Click(new Rect(175,y,91,29),"Cue cũ",mode==2 ? pickedStyle:buttonStyle)) SwitchMode(2);
        y+=40;
        if(!string.IsNullOrEmpty(Variant.prior))
        {
            if(Click(new Rect(0,y,265,27),"So sánh bản phối trước",mode==-1 ? pickedStyle : buttonStyle)) SwitchMode(-1);
            y+=34;
        }
        PaintText(new Rect(0,y,267,24),"Lớp âm thanh",accentStyle);
        y+=25;
        for(int i=0;i<(Variant.layers?.Length??0);i++)
        {
            if(Click(new Rect(0,y,265,27),Variant.layers[i].label,mode==i+3 ? pickedStyle : buttonStyle)) SwitchMode(i+3);
            y+=32;
        }
        if(Variant.layers==null || Variant.layers.Length==0)
        {
            PaintText(new Rect(0,y,267,20),"Cue đơn",smallStyle);
            y+=26;
        }
        PaintText(new Rect(0,y,267,19),"Tệp  ·  "+Path.GetFileName(FileReference()),smallStyle); y+=22;
        PaintText(new Rect(0,y,267,23),"Nhóm  ·  Preview / "+(Track ? "Music" : "SFX"),smallStyle); y+=23;
        string integration=current.branch.Contains("[A/P]") || current.branch.Contains("[P]") ? "Đề xuất cơ chế" : "Cue mới trong Studio";
        PaintText(new Rect(0,y,267,22),$"{integration}  ·  {current.priority}",smallStyle); y+=24;
        PaintText(new Rect(0,y,267,22),ReviewStatus(current),accentStyle); y+=26;
        int mark=PlayerPrefs.GetInt(ReviewKey);
        if(Click(new Rect(0,y,85,28),"Chờ duyệt",mark==0 ? pickedStyle:buttonStyle)) SetReview(0);
        if(Click(new Rect(91,y,82,28),"Đạt",mark==1 ? pickedStyle:buttonStyle)) SetReview(1);
        if(Click(new Rect(179,y,86,28),"Cần chỉnh",mark==2 ? pickedStyle:buttonStyle)) SetReview(2);
        y+=39;
        PaintText(new Rect(0,y,267,20),"Nhận xét theo biến thể",accentStyle); y+=22;
        GUI.SetNextControlName("studio-note");
        reviewNote=GUI.TextArea(new Rect(0,y,264,64),reviewNote,1200);
        y+=68;
        if(Click(new Rect(0,y,130,27),"Lưu nhận xét"))
        {
            SaveReviewNote();
            GUI.FocusControl(null);
        }
        PaintText(new Rect(139,y+4,126,20),PlayerPrefs.GetString(CommentKey)==reviewNote ? "Đã lưu" : "Chưa lưu",smallStyle);
        y+=34;
        PaintText(new Rect(0,y,267,61),stage.VisualNote,smallStyle); y+=67;
        PaintText(new Rect(0,y,267,80),current.timing,smallStyle); y+=83;
        string sources=current.sources?.Length>0 ? "Kenney CC0 · "+string.Join(", ",current.sources) : "Âm thanh nguyên bản.";
        if(current.projectLayers?.Length>0) sources+="\nWAV dự án · "+string.Join(", ",current.projectLayers);
        PaintText(new Rect(0,y,267,105),sources,smallStyle);
        GUI.EndScrollView();
    }

    private void Level(float x,float y,float width,string label,ref float value,float min,float max,bool percent)
    {
        string text=percent ? $"{label}   {value:P0}" : $"{label}   {value:0.00}";
        PaintText(new Rect(x,y,width,21),text,smallStyle);
        value=GUI.HorizontalSlider(new Rect(x,y+21,width,17),value,min,max);
    }

    private void Toggle(Rect rect,string text,ref bool flag)
    {
        if(Click(rect,flag ? text+" ✓" : text,flag ? pickedStyle:buttonStyle)) flag=!flag;
    }

    private void Shortcuts()
    {
        var input=Event.current;
        if(input.type!=EventType.KeyDown || GUI.GetNameOfFocusedControl() is "studio-search" or "studio-note") return;
        if(input.keyCode==KeyCode.Space)
        {
            if(sound.Playing || sound.Loading || pendingSource) StopCue(); else PlayCue();
            input.Use();
        }
        else if(input.keyCode is KeyCode.LeftArrow or KeyCode.RightArrow)
        {
            StepVariant(input.keyCode==KeyCode.LeftArrow ? -1 : 1);
            input.Use();
        }
    }

    private void SetReview(int value)
    {
        PlayerPrefs.SetInt(ReviewKey,value);
        PlayerPrefs.Save();
    }

    private static string Fold(string text)
    {
        var buffer=new StringBuilder();
        foreach(char ch in text.ToLowerInvariant().Normalize(NormalizationForm.FormD))
            if(CharUnicodeInfo.GetUnicodeCategory(ch)!=UnicodeCategory.NonSpacingMark)
                buffer.Append(ch=='đ'?'d':ch);
        return buffer.ToString();
    }

    private bool Click(Rect rect,string text,GUIStyle style=null) => GUI.Button(rect,text,style??buttonStyle);
    private static void PaintText(Rect rect,string text,GUIStyle style) => GUI.Label(rect,text??"",style);

    private static void Box(Rect rect)
    {
        Fill(rect,Border);
        Fill(new Rect(rect.x+1,rect.y+1,rect.width-2,rect.height-2),Panel);
    }

    private static void Fill(Rect rect,Color color)
    {
        Color old=GUI.color;
        GUI.color=color;
        GUI.DrawTexture(rect,Texture2D.whiteTexture);
        GUI.color=old;
    }

    private void OnDestroy()
    {
        foreach (var texture in new[] {buttonTexture,buttonOver,pickedTexture,playTexture,playOver,rowTexture})
            if(texture!=null) Destroy(texture);
    }
}
