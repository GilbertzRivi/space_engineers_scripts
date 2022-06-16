int distance = 450;
int objsize = 7;
int rotationdeg = 45;
float maxbsheight = 7.5f;
Color radarcolor = Color.Green;

List<IMyCameraBlock> CamsR0 = new List<IMyCameraBlock>();
List<IMyCameraBlock> CamsR1 = new List<IMyCameraBlock>();
List<List<IMyCameraBlock>> CamRows = new List<List<IMyCameraBlock>>();

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
    IMyTextPanel LCD = GridTerminalSystem.GetBlockWithName("LCD Radar") as IMyTextPanel;
    IMyMotorStator Rotor = GridTerminalSystem.GetBlockWithName("Rotor Radar Main") as IMyMotorStator;
    CamsR0.Clear();
    GridTerminalSystem.GetBlocksOfType<IMyCameraBlock>(CamsR0, c => c.CustomName.Contains("Camera R0"));
    CamsR0.Sort((x, y) => x.CustomName[x.CustomName.Length - 1].CompareTo(y.CustomName[x.CustomName.Length - 1]));
    CamsR0.Reverse();

    CamsR1.Clear();
    GridTerminalSystem.GetBlocksOfType<IMyCameraBlock>(CamsR1, c => c.CustomName.Contains("Camera R1"));
    CamsR1.Sort((x, y) => x.CustomName[x.CustomName.Length - 1].CompareTo(y.CustomName[x.CustomName.Length - 1]));
    CamsR1.Reverse();

    CamRows.Clear();
    CamRows.Add(CamsR0);
    CamRows.Add(CamsR1);

    Vector2 Resolution = LCD.SurfaceSize;
    float Angle = Rotor.Angle;
    int ObjLifetime = (int)(Rotor.TargetVelocityRPM / CamsR0.Count * 360 - 10);

    for (int row = 0; row < CamRows.Count; row++) {
        for (int i = 0; i < CamsR0.Count; i++)
        {
            var cam = CamsR0[i];
            cam.EnableRaycast = true;
            float pitch = (maxbsheight / distance) * (float)(180 / Math.PI) * row;
            Echo(pitch.ToString());
            MyDetectedEntityInfo RC = cam.Raycast(distance, pitch, rotationdeg);
            Vector3D CamPos = cam.GetPosition();

            if (RC.HitPosition.HasValue)
            {
                Vector3D HitPos = RC.HitPosition.Value;
                double Distance = DistanceCalculation(CamPos, HitPos);
                int PosX = (int)((Distance * Math.Cos(Angle + Math.PI / (0.5 * CamsR0.Count) * i) / distance * Resolution.X / 2) + Resolution.X / 2);
                int PosY = (int)((Distance * Math.Sin(Angle + Math.PI / (0.5 * CamsR0.Count) * i) / distance * Resolution.Y / 2) + Resolution.Y / 2);
                int PosDistance = (int)Math.Sqrt(Math.Pow(PosX - 256, 2) + Math.Pow(PosY - 256, 2));
                if (PosDistance <= 224)
                {
                    DetectedObject obj = new DetectedObject(new Vector2(PosX, PosY), ObjLifetime);
                    DetectedObjects.Add(obj);
                }
            }
        }
    }


    using (var Frame = LCD.DrawFrame())
    {
        DrawShape(Frame, 224, 12, new Vector2(256), 1, radarcolor);
        DrawShape(Frame, 168, 12, new Vector2(256), 1, radarcolor);
        DrawShape(Frame, 112, 12, new Vector2(256), 1, radarcolor);
        DrawShape(Frame, 56, 12, new Vector2(256), 1, radarcolor);
        DrawLine(Frame, new Vector2(256, 32), new Vector2(256, 480), 1, radarcolor);
        DrawLine(Frame, new Vector2(32, 256), new Vector2(480, 256), 1, radarcolor);

        for (int i = 0; i < CamsR0.Count; i++)
        {
            Vector2 LineStart = new Vector2(256);
            Vector2 LineEnd = new Vector2((int)(224 * Math.Sin(-(Angle - Math.PI / (0.5 * CamsR0.Count) * i)) + 256),
                (int)(224 * Math.Cos(-(Angle - Math.PI / (0.5 * CamsR0.Count) * i))) + 256);
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
