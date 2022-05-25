public Program(){
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Main(string argument, UpdateType updateType){
    IMyBlockGroup magazyn_group = GridTerminalSystem.GetBlockGroupWithName("cargo");
    List<IMyCargoContainer> cargo = new List<IMyCargoContainer>();
    magazyn_group.GetBlocksOfType(cargo); 
    IMyTextSurface LCD = Me.GetSurface(0);
    LCD.ContentType = ContentType.TEXT_AND_IMAGE;
    LCD.FontSize = (float)0.8;
    LCD.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;
    LCD.TextPadding = (float)32;
    
    float usedVolume = 0;
    float maxVolume = 0;

    for(int i = 0; i < cargo.Count; i++){
        var cargo_b = cargo[i];
        usedVolume += (float)cargo_b.GetInventory(0).CurrentVolume;   
        maxVolume += (float)cargo_b.GetInventory(0).MaxVolume;   
    }
    
    int procenty = (int)Math.Round(Math.Round(usedVolume)/Math.Round(maxVolume)*40);
    int procenty_real = (int)Math.Round(Math.Round(usedVolume)/Math.Round(maxVolume)*100);
    string text = "Sprawdzanie kontenerów w grupie 'cargo'\nIlość kontenerów: " + cargo.Count.ToString();
    text += "\nPoziom zapełnienia magazynu:\n";
    text +=  new String('#', procenty);
    text += new String('=', (40 - procenty));
    text += "\n" + procenty_real.ToString() + "%";

    LCD.WriteText(text);

}
