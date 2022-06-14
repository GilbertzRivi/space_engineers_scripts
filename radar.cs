int resolution = 70;
char[,] DisplayTable = new char[70 + 1, 70 + 1];
double DistanceCalculation(Vector3D CamPos, Vector3D HitPos)
{
    double DistanceX = (double)(CamPos.X - HitPos.X);
    double DistanceY = (double)(CamPos.Y - HitPos.Y);
    double DistanceZ = (double)(CamPos.Z - HitPos.Z);
    double Distance = Math.Sqrt(Math.Pow(DistanceX, 2) + Math.Pow(DistanceY, 2) + Math.Pow(DistanceZ, 2));
    return Distance;
}

bool LinePointIntersection(double PointX, double PointY, double AngleDeg)
{
    AngleDeg = Math.Round(AngleDeg);
    double AngleRad = AngleDeg * (Math.PI / 180);
    if (PointX == 0 & PointY == 0) { return true; }
    if (PointX == 0)
    {
        if (PointY > 0 & AngleDeg == 90) { return true; }
        else if (PointY < 0 & AngleDeg == 270) { return true; }
        else { return false; }
    }
    else if (PointY == 0)
    {
        if (PointX > 0 & AngleDeg == 0) { return true; }
        if (PointX < 0 & AngleDeg == 180) { return true; }
        else { return false; }
    }
    else
    {
        if(AngleRad <= Math.PI)
        {
            if (Math.Abs(AngleRad - Math.Atan2(PointY, PointX)) < 0.015) { return true; }
            else { return false; }
        }
        else
        {
            if (Math.Abs(AngleRad - ((Math.PI * 2) + Math.Atan2(PointY, PointX))) < 0.015) { return true; }
            else { return false; }
        }
    }
}

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public void Main(string argument, UpdateType updateSource)
{
    IMyTextSurface LCD = GridTerminalSystem.GetBlockWithName("LCD Radar") as IMyTextSurface;
    IMyMotorStator Rotor = GridTerminalSystem.GetBlockWithName("Rotor Radar") as IMyMotorStator;
    IMyCameraBlock Cam = GridTerminalSystem.GetBlockWithName("Camera c0 r0") as IMyCameraBlock;

    Rotor.TargetVelocityRPM = (float)-2;
    Cam.EnableRaycast = true;

    MyDetectedEntityInfo RC = Cam.Raycast((double)250);

    Vector3D CamPos = Cam.GetPosition();

    if (RC.HitPosition.HasValue)
    {

        Vector3D HitPos = RC.HitPosition.Value;
        double Distance = DistanceCalculation(CamPos, HitPos);

        if (Distance <= 250)
        {
            double Angle = Rotor.Angle;
            int DisplayX = (int)Math.Round(Distance * Math.Cos(Angle) / 250 * resolution) + resolution/2;
            int DisplayY = (int)Math.Round(Distance * Math.Sin(Angle) / 250 * resolution) + resolution/2;
            LCD.WriteText(DisplayX.ToString() + " " + DisplayY.ToString());
            if (DisplayX < resolution & DisplayX > 0 & DisplayY < resolution & DisplayY > 0)
            {
                DisplayTable[DisplayX, DisplayY] = '#';
            }
        }
    }

    for (int y = resolution/2; y > -resolution/2; y--)
    {
        for (int x = -resolution/2; x < resolution/2; x++)
        {
            if (DisplayTable[x + resolution/2, y + resolution/2] != '#')
            {
                if (LinePointIntersection(x, y, Rotor.Angle * (180 / Math.PI))) { DisplayTable[x + resolution/2, y + resolution/2] = '0'; }
                else { DisplayTable[x + resolution/2, y + resolution/2] = '-'; }
            }
        }
    }
    string output = "";
    for (int x = 0; x < DisplayTable.GetLength(0); x++)
    {
        for (int y = 0; y < DisplayTable.GetLength(1); y++)
        {
            output += DisplayTable[x, y];
        }
        output += "\n";
    }

    LCD.WriteText(output);

}
