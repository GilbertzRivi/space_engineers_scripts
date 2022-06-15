int distance = 450;
int objsize = 7;
Color radarcolor = Color.Green;

List<DetectedObject> DetectedObjects = new List<DetectedObject>();
public class DetectedObject
{
    public Vector2 ScreenPosition { get; set; }
    public bool Showed { get; set; }
    public int Lifetime { get; set; }

    public DetectedObject(Vector2 ScreenPosition, int Lifetime)
    {
        this.ScreenPosition = ScreenPosition;
        this.Showed = false;
        this.Lifetime = Lifetime;
    }
}

double DistanceCalculation(Vector3D CamPos, Vector3D HitPos)
{
    double DistanceX = (double)(CamPos.X - HitPos.X);
    double DistanceY = (double)(CamPos.Y - HitPos.Y);
    double DistanceZ = (double)(CamPos.Z - HitPos.Z);
    double Distance = Math.Sqrt(Math.Pow(DistanceX, 2) + Math.Pow(DistanceY, 2) + Math.Pow(DistanceZ, 2));
    return Distance;
}
void DrawLine(MySpriteDrawFrame Frame, Vector2 LineStart, Vector2 LineEnd, int width, Color color)
{
    Vector2 position = 0.5f * (LineStart + LineEnd);
    Vector2 diff = LineStart - LineEnd;
    float length = diff.Length();
    if (length > 0)
        diff /= length;

    Vector2 size = new Vector2(length, width);
    float angle = (float)Math.Acos(Vector2.Dot(diff, Vector2.UnitX));
    angle *= Math.Sign(Vector2.Dot(diff, Vector2.UnitY));

    MySprite sprite = MySprite.CreateSprite("SquareSimple", position, size);
    sprite.RotationOrScale = angle;
    sprite.Color = color;
    Frame.Add(sprite);
}

void DrawShape(MySpriteDrawFrame Frame, int radius, int step, Vector2 offset, int width, Color color)
{
    for (int i = 0; i <= 360; i += step)
    {
        Vector2 LineStart = new Vector2((int)(radius * Math.Sin(Math.PI * 2 * i / 360) + offset.X),
            (int)(radius * Math.Cos(Math.PI * 2 * i / 360)) + offset.Y);
        Vector2 LineEnd = new Vector2((int)(radius * Math.Sin(Math.PI * 2 * (i + step) / 360) + offset.X),
            (int)(radius * Math.Cos(Math.PI * 2 * (i + step) / 360)) + offset.Y);

        DrawLine(Frame, LineStart, LineEnd, width, color);
    }
}
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}
public void Main(string argument, UpdateType updateSource)
{
    IMyTextPanel test = GridTerminalSystem.GetBlockWithName("LCD test") as IMyTextPanel;
    IMyMotorStator Rotor = GridTerminalSystem.GetBlockWithName("Rotor Radar") as IMyMotorStator;
    IMyCameraBlock Camc0r0 = GridTerminalSystem.GetBlockWithName("Camera c0 r0") as IMyCameraBlock;
    IMyCameraBlock Camc1r0 = GridTerminalSystem.GetBlockWithName("Camera c1 r0") as IMyCameraBlock;
    IMyCameraBlock Camc2r0 = GridTerminalSystem.GetBlockWithName("Camera c2 r0") as IMyCameraBlock;
    IMyCameraBlock Camc3r0 = GridTerminalSystem.GetBlockWithName("Camera c3 r0") as IMyCameraBlock;
    List<IMyCameraBlock> Cams = new List<IMyCameraBlock>() { Camc0r0, Camc1r0, Camc2r0, Camc3r0 };

    Vector2 Resolution = test.SurfaceSize;
    float Angle = Rotor.Angle;
    int ObjLifetime = (int)(Rotor.TargetVelocityRPM / 4 * 360 - 10);

    for (int i = 0; i < Cams.Count; i++)
    {
        var cam = Cams[i];
        cam.EnableRaycast = true;
        MyDetectedEntityInfo RC = cam.Raycast(distance);
        Vector3D CamPos = cam.GetPosition();

        if (RC.HitPosition.HasValue)
        {
            Vector3D HitPos = RC.HitPosition.Value;
            double Distance = DistanceCalculation(CamPos, HitPos);
            int PosX = (int)((Distance * Math.Cos(Angle + Math.PI / 2 * i) / distance * Resolution.X / 2) + Resolution.X / 2);
            int PosY = (int)((Distance * Math.Sin(Angle + Math.PI / 2 * i) / distance * Resolution.Y / 2) + Resolution.Y / 2);
            int PosDistance = (int)Math.Sqrt(Math.Pow(PosX - 256, 2) + Math.Pow(PosY - 256, 2));
            if (PosDistance <= 224)
            {
                DetectedObject obj = new DetectedObject(new Vector2(PosX, PosY), ObjLifetime);
                DetectedObjects.Add(obj);
            }
        }
    }

    using (var Frame = test.DrawFrame())
    {
        DrawShape(Frame, 224, 12, new Vector2(256), 1, radarcolor);
        DrawShape(Frame, 168, 12, new Vector2(256), 1, radarcolor);
        DrawShape(Frame, 112, 12, new Vector2(256), 1, radarcolor);
        DrawShape(Frame, 56, 12, new Vector2(256), 1, radarcolor);
        DrawLine(Frame, new Vector2(256, 32), new Vector2(256, 480), 1, radarcolor);
        DrawLine(Frame, new Vector2(32, 256), new Vector2(480, 256), 1, radarcolor);

        for (int i = 0; i < 4; i++)
        {
            Vector2 LineStart = new Vector2(256);
            Vector2 LineEnd = new Vector2((int)(224 * Math.Sin(-(Angle - Math.PI / 2 * i)) + 256), (int)(224 * Math.Cos(-(Angle - Math.PI / 2 * i))) + 256);
            DrawLine(Frame, LineStart, LineEnd, 1, radarcolor);
        }

        List<DetectedObject> DetectedObjectsCopy = new List<DetectedObject>(DetectedObjects);

        foreach (var obj in DetectedObjectsCopy)
        {
            if (obj.Lifetime == 0)
            {
                DetectedObjects.Remove(obj);
                continue;
            }
            MySprite ObjSprite = MySprite.CreateSprite("Circle", obj.ScreenPosition, new Vector2(objsize));
            ObjSprite.Color = radarcolor;
            Frame.Add(ObjSprite);
            obj.Lifetime -= 1;
        }

    }
}
