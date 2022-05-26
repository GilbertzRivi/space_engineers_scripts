public Program(){
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public string string_reverse(string s){
    char[] charArray = s.ToCharArray();
    Array.Reverse(charArray);
    return new string(charArray);
}

public string text_format(string s){
    string formated = "";
    s = string_reverse(s);
    for(int i = 0; i < s.Length; i++){
        if(i % 3 == 0){
            formated += " ";
        }
        formated += s[i];
    }
    formated = string_reverse(formated);
    return formated.Remove(formated.Length -1);
}

public void Main(string argument, UpdateType updateType){
    IMyBlockGroup magazyn_group = GridTerminalSystem.GetBlockGroupWithName("cargo średnie gówno");
    List<IMyCargoContainer> cargo = new List<IMyCargoContainer>();
    magazyn_group.GetBlocksOfType(cargo); 
    IMyCockpit cockpit = GridTerminalSystem.GetBlockWithName("Cockpit średnie gówno") as IMyCockpit;
    IMyBlockGroup drill_group = GridTerminalSystem.GetBlockGroupWithName("drille średnie gówno");
    List<IMyShipDrill> drille = new List<IMyShipDrill>();
    drill_group.GetBlocksOfType(drille);
    IMyTextSurface LCD = cockpit.GetSurface(0);
    LCD.ContentType = ContentType.TEXT_AND_IMAGE;
    LCD.FontSize = (float)2;
    LCD.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;
    LCD.TextPadding = (float)30;
    
    float usedVolume = 0;
    float maxVolume = 0;

    for(int i = 0; i < cargo.Count; i++){
        var cargo_b = cargo[i];
        usedVolume += (float)cargo_b.GetInventory(0).CurrentVolume;   
        maxVolume += (float)cargo_b.GetInventory(0).MaxVolume;   
    }
    
    int procenty = (int)Math.Round(Math.Round(usedVolume)/Math.Round(maxVolume)*10);
    int procenty_real = (int)Math.Round(Math.Round(usedVolume)/Math.Round(maxVolume)*100);
    string text =  new String('#', procenty);
    text += new String('=', (10 - procenty));
    text += "\n" + procenty_real.ToString() + "%";

    string used_volume_txt = text_format(Math.Round(usedVolume*1000).ToString());
    string max_volume_txt = text_format(Math.Round(maxVolume*1000).ToString());

    text += "\n" + used_volume_txt + "/" + max_volume_txt + " L";

    LCD.WriteText(text);

    if(usedVolume >= maxVolume){
        foreach(var drill in drille){
            drill.Enabled = false;
        }
    }

}
