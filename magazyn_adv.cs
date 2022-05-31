
List<IMyTextPanel> lcd_cargo_main = new List<IMyTextPanel>();
List<IMyTextPanel> lcd_cargo_ore = new List<IMyTextPanel>();
List<IMyTextPanel> lcd_ice = new List<IMyTextPanel>();
List<IMyTextPanel> lcd_h2 = new List<IMyTextPanel>();
List<IMyTextPanel> lcd_o2 = new List<IMyTextPanel>();
List<IMyGasTank> tanks_h2 = new List<IMyGasTank>();
List<IMyGasTank> tanks_o2 = new List<IMyGasTank>();
List<IMyCargoContainer> cargo_main = new List<IMyCargoContainer>();
List<IMyCargoContainer> cargo_ore = new List<IMyCargoContainer>();
List<IMyCargoContainer> cargo_ice = new List<IMyCargoContainer>();

public Program(){
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
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

public string text_compose(float used_volume, float max_volume, List<IMyCargoContainer> containers, string type, int loading_wide){

    int loading_bar = (int)Math.Round(Math.Round(used_volume)/Math.Round(max_volume)*loading_wide);
    int percent = (int)Math.Round(Math.Round(used_volume)/Math.Round(max_volume)*100);
    string text = "Ilość kontenerów: " + containers.Count.ToString();
    text += "\nPoziom zapełnienia magazynu " + type + ":\n";
    text += "[" + new String('|', loading_bar);
    text += new String(Convert.ToChar("'"), (loading_wide - loading_bar)) + "]";
    text += "\n" + percent.ToString() + "%";

    string used_volume_txt = text_format(Math.Round(used_volume*1000).ToString());
    string max_volume_txt = text_format(Math.Round(max_volume*1000).ToString());

    text += "\n" + used_volume_txt + "/" + max_volume_txt + " L";
    return text;
}

public string text_compose_tanks(float used_volume, float max_volume, List<IMyGasTank> tanks, string type, int loading_wide){
    int loading_bar = (int)Math.Round(Math.Round(used_volume)/Math.Round(max_volume)*loading_wide);
    int percent = (int)Math.Round(Math.Round(used_volume)/Math.Round(max_volume)*100);
    string text = "Ilość kontenerów: " + tanks.Count.ToString();
    text += "\nPoziom zapełnienia zbiorników " + type + ":\n";
    text += "[" + new String('|', loading_bar);
    text += new String(Convert.ToChar("'"), (loading_wide - loading_bar)) + "]";
    text += "\n" + percent.ToString() + "%";

    string used_volume_txt = text_format(Math.Round(used_volume).ToString());
    string max_volume_txt = text_format(Math.Round(max_volume).ToString());

    text += "\n" + used_volume_txt + "/" + max_volume_txt + " L";
    return text;
}

public MyTuple<float, float> cargo_volume(List<IMyCargoContainer> cargo){
    
    float usedVolume = 0;
    float maxVolume = 0;
    for(int i = 0; i < cargo.Count(); i++){
        var container = cargo[i];
        usedVolume += (float)container.GetInventory(0).CurrentVolume;
        maxVolume += (float)container.GetInventory(0).MaxVolume;
    }
    return MyTuple.Create(usedVolume, maxVolume);
}

public MyTuple<float, float> tanks_volume(List<IMyGasTank> tanks){
    
    float usedVolume = 0;
    float maxVolume = 0;
    for(int i = 0; i < tanks.Count(); i++){
        var tank = tanks[i];
        maxVolume += (float)tank.Capacity;
        usedVolume += (float)tank.FilledRatio * (float)tank.Capacity;
    }
    return MyTuple.Create(usedVolume, maxVolume);
}

public void Main(string argument, UpdateType updateType){
    GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargo_main, c => c.CustomName.Contains("[MAIN]"));
    GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargo_ore, c => c.CustomName.Contains("[ORE"));
    GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargo_ice, c => c.CustomName.Contains("[ICE]"));
    GridTerminalSystem.GetBlocksOfType<IMyGasTank>(tanks_h2, t => t.CustomName.Contains("[H2]"));
    GridTerminalSystem.GetBlocksOfType<IMyGasTank>(tanks_o2, t => t.CustomName.Contains("[O2]"));

    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcd_cargo_main, p => p.CustomName == "LCD CARGO MAIN");
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcd_cargo_ore, p => p.CustomName == "LCD CARGO ORE");
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcd_ice, p => p.CustomName == "LCD ICE");
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcd_h2, p => p.CustomName == "LCD HYDROGEN");
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcd_o2, p => p.CustomName == "LCD OXYGEN");   

    IMyTextSurface lcd_main = GridTerminalSystem.GetBlockWithName("LCD STORAGE") as IMyTextSurface;

    string text = "";
    
    var volume_main = cargo_volume(cargo_main);
    text = text_compose(volume_main.Item1, volume_main.Item2, cargo_main, "[MAIN]", 80);
    foreach(var lcd in lcd_cargo_main){
        lcd.WriteText(text);
    }

    var volume_ore = cargo_volume(cargo_ore);
    text = text_compose(volume_ore.Item1, volume_ore.Item2, cargo_ore, "[ORE]", 80);
    foreach(var lcd in lcd_cargo_ore){
        lcd.WriteText(text);
    }

    var volume_ice = cargo_volume(cargo_ice);
    text = text_compose(volume_ice.Item1, volume_ice.Item2, cargo_ice, "[ICE]", 80);
    foreach(var lcd in lcd_ice){
        lcd.WriteText(text);
    }

    var volume_h2 = tanks_volume(tanks_h2);
    text = text_compose_tanks(volume_h2.Item1, volume_h2.Item2, tanks_h2, "[HYDROGEN]", 80);
    foreach(var lcd in lcd_h2){
        lcd.WriteText(text);
    }

    var volume_o2 = tanks_volume(tanks_o2);
    text = text_compose_tanks(volume_o2.Item1, volume_o2.Item2, tanks_o2, "[OXYGEN]", 80);
    foreach(var lcd in lcd_o2){
        lcd.WriteText(text);
    }

    string programmable_text = "";

    programmable_text += "Cargo Main: " + text_format(Math.Round(volume_main.Item1*1000).ToString());
    programmable_text += "/" + text_format(Math.Round(volume_main.Item2*1000).ToString()) + "L";
    programmable_text += " " + (int) Math.Round(volume_main.Item1/volume_main.Item2*100) + "%\n";
    programmable_text += new String('#', ((int)(volume_main.Item1/volume_main.Item2*30)));
    programmable_text += new String('=', (30 - (int)(volume_main.Item1/volume_main.Item2*30))) + "\n";

    programmable_text += "Cargo Ore: " + text_format(Math.Round(volume_ore.Item1*1000).ToString());
    programmable_text += "/" + text_format(Math.Round(volume_ore.Item2*1000).ToString()) + "L";
    programmable_text += " " + (int) Math.Round(volume_ore.Item1/volume_ore.Item2*100) + "%\n";
    programmable_text += new String('#', ((int)(volume_ore.Item1/volume_ore.Item2*30)));
    programmable_text += new String('=', (30 - (int)(volume_ore.Item1/volume_ore.Item2*30))) + "\n";

    programmable_text += "Cargo Ice: " + text_format(Math.Round(volume_ice.Item1*1000).ToString());
    programmable_text += "/" + text_format(Math.Round(volume_ice.Item2*1000).ToString()) + "L";
    programmable_text += " " + (int) Math.Round(volume_ice.Item1/volume_ice.Item2*100) + "%\n";
    programmable_text += new String('#', ((int)(volume_ice.Item1/volume_ice.Item2*30)));
    programmable_text += new String('=', (30 - (int)(volume_ice.Item1/volume_ice.Item2*30))) + "\n";

    programmable_text += "Cargo H2: " + text_format(Math.Round(volume_h2.Item1).ToString());
    programmable_text += "/" + text_format(Math.Round(volume_h2.Item2).ToString()) + "L";
    programmable_text += " " + (int) Math.Round(volume_h2.Item1/volume_h2.Item2*100) + "%\n";
    programmable_text += new String('#', ((int)(volume_h2.Item1/volume_h2.Item2*30)));
    programmable_text += new String('=', (30 - (int)(volume_h2.Item1/volume_h2.Item2*30))) + "\n";

    programmable_text += "Cargo O2: " + text_format(Math.Round(volume_o2.Item1).ToString());
    programmable_text += "/" + text_format(Math.Round(volume_o2.Item2).ToString()) + "L";
    programmable_text += " " + (int) Math.Round(volume_o2.Item1/volume_o2.Item2*100) + "%\n";
    programmable_text += new String('#', ((int)(volume_o2.Item1/volume_o2.Item2*30)));
    programmable_text += new String('=', (30 - (int)(volume_o2.Item1/volume_o2.Item2*30))) + "\n";

    lcd_main.WriteText(programmable_text);

}
