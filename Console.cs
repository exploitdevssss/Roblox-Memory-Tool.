using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.IO;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace rbx_external
{
    // ─────────────────────────────────────────────
    //  OFFSETS
    // ─────────────────────────────────────────────
    public class offsets
    {
        public static int Adornee; public static int Anchored; public static int AnchoredMask;
        public static int AnimationId; public static int AttributeToNext; public static int AttributeToValue;
        public static int AutoJumpEnabled; public static int BanningEnabled; public static int BeamBrightness;
        public static int BeamColor; public static int BeamLightEmission; public static int BeamLightInfuence;
        public static int CFrame; public static int Camera; public static int CameraMaxZoomDistance;
        public static int CameraMinZoomDistance; public static int CameraMode; public static int CameraPos;
        public static int CameraRotation; public static int CameraSubject; public static int CameraType;
        public static int CanCollide; public static int CanCollideMask; public static int CanTouch;
        public static int CanTouchMask; public static int CharacterAppearanceId; public static int Children;
        public static int ChildrenEnd; public static int ClassDescriptor; public static int ClassDescriptorToClassName;
        public static int ClickDetectorMaxActivationDistance; public static int ClockTime; public static int CreatorId;
        public static int DataModelDeleterPointer; public static int DataModelPrimitiveCount;
        public static int DataModelToRenderView1; public static int DataModelToRenderView2;
        public static int DataModelToRenderView3; public static int DecalTexture; public static int Deleter;
        public static int DeleterBack; public static int Dimensions; public static int DisplayName;
        public static int EvaluateStateMachine; public static int FFlagList; public static int FFlagToValueGetSet;
        public static int FOV; public static int FakeDataModelPointer; public static int FakeDataModelToDataModel;
        public static int FogColor; public static int FogEnd; public static int FogStart;
        public static int ForceNewAFKDuration; public static int FramePositionOffsetX;
        public static int FramePositionOffsetY; public static int FramePositionX; public static int FramePositionY;
        public static int FrameRotation; public static int FrameSizeOffsetX; public static int FrameSizeOffsetY;
        public static int FrameSizeX; public static int FrameSizeY; public static int FrameVisible;
        public static int GameId; public static int GameLoaded; public static int Gravity;
        public static int Health; public static int HealthDisplayDistance; public static int HipHeight;
        public static int HumanoidDisplayName; public static int HumanoidState; public static int HumanoidStateId;
        public static int InputObject; public static int InsetMaxX; public static int InsetMaxY;
        public static int InsetMinX; public static int InsetMinY; public static int InstanceAttributePointer1;
        public static int InstanceAttributePointer2; public static int InstanceCapabilities;
        public static int JobEnd; public static int JobId; public static int JobStart; public static int Job_Name;
        public static int JobsPointer; public static int JumpPower; public static int LocalPlayer;
        public static int LocalScriptByteCode; public static int LocalScriptBytecodePointer;
        public static int LocalScriptHash; public static int MaterialType; public static int MaxHealth;
        public static int MaxSlopeAngle; public static int MeshPartColor3; public static int MeshPartTexture;
        public static int ModelInstance; public static int ModuleScriptByteCode;
        public static int ModuleScriptBytecodePointer; public static int ModuleScriptHash;
        public static int MoonTextureId; public static int MousePosition; public static int MouseSensitivity;
        public static int MoveDirection; public static int Name; public static int NameDisplayDistance;
        public static int NameSize; public static int OnDemandInstance; public static int OutdoorAmbient;
        public static int Parent; public static int PartSize; public static int Ping; public static int PlaceId;
        public static int PlayerConfigurerPointer; public static int PlayerMouse; public static int Position;
        public static int Primitive; public static int PrimitiveValidateValue;
        public static int PrimitivesPointer1; public static int PrimitivesPointer2;
        public static int ProximityPromptActionText; public static int ProximityPromptEnabled;
        public static int ProximityPromptGamepadKeyCode; public static int ProximityPromptHoldDuraction;
        public static int ProximityPromptMaxActivationDistance; public static int ProximityPromptMaxObjectText;
        public static int ReadOnlyGravity; public static int RenderJobToDataModel;
        public static int RenderJobToFakeDataModel; public static int RenderJobToRenderView;
        public static int RequireBypass; public static int RigType; public static int RootPartR15;
        public static int RootPartR6; public static int Rotation; public static int RunContext;
        public static int Sandboxed; public static int ScreenGuiEnabled; public static int ScriptContext;
        public static int Sit; public static int SkyboxBk; public static int SkyboxDn; public static int SkyboxFt;
        public static int SkyboxLf; public static int SkyboxRt; public static int SkyboxUp;
        public static int SoundId; public static int StarCount; public static int StringLength;
        public static int SunTextureId; public static int TagList; public static int TaskSchedulerMaxFPS;
        public static int TaskSchedulerPointer; public static int Team; public static int TeamColor;
        public static int TextLabelText; public static int TextLabelVisible; public static int Tool_Grip_Position;
        public static int Transparency; public static int UserId; public static int Value;
        public static int ValueGetSetToValue; public static int Velocity; public static int ViewportSize;
        public static int VisualEngine; public static int VisualEnginePointer;
        public static int VisualEngineToDataModel1; public static int VisualEngineToDataModel2;
        public static int WalkSpeed; public static int WalkSpeedCheck; public static int Workspace;
        public static int WorkspaceToWorld; public static int viewmatrix;

        // Human-readable status shown in the menu after load
        public static string UpdateStatus = "unknown";
        public static string LoadedVersion = "";

        const string UPDATE_URL = "https://offsets.ntgetwritewatch.workers.dev/offsets.json";

        // Reads the copy of offsets.json baked into the exe at build time
        static string ReadEmbedded()
        {
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            string resName = asm.GetName().Name + ".offsets.json";
            using (var stream = asm.GetManifestResourceStream(resName))
            {
                if (stream == null) return "";
                using (var reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
        }

        // Pulls the RobloxVersion string from raw JSON without fully deserialising
        static string ParseVersion(string json)
        {
            try
            {
                using (var doc = JsonDocument.Parse(json))
                    if (doc.RootElement.TryGetProperty("RobloxVersion", out var v))
                        return v.GetString() ?? "";
            }
            catch { }
            return "";
        }

        // Disk cache path — sits next to the exe so VS debug and release both find it
        static string CachePath() =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "offsets_cache.json");

        public static async Task LoadAsync()
        {
            // ── 1. Gather candidates ─────────────────────────────────────────
            string embeddedJson = ReadEmbedded();
            string embeddedVer  = ParseVersion(embeddedJson);

            // Try disk cache (written by a previous successful fetch)
            string cacheJson = "";
            string cacheVer  = "";
            if (File.Exists(CachePath()))
            {
                cacheJson = File.ReadAllText(CachePath());
                cacheVer  = ParseVersion(cacheJson);
            }

            // ── 2. Fetch live offsets and compare versions ────────────────────
            string liveJson = "";
            string liveVer  = "";
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(6);
                    client.DefaultRequestHeaders.Add("User-Agent", "rbx-external/1.0");
                    liveJson = await client.GetStringAsync(UPDATE_URL);
                    liveVer  = ParseVersion(liveJson);
                }
            }
            catch { /* no network — fall through */ }

            // ── 3. Decide which JSON to use ───────────────────────────────────
            string finalJson = "";

            if (!string.IsNullOrWhiteSpace(liveJson))
            {
                // Check whether the live version is actually newer than what we have
                bool newerThanEmbedded = !string.IsNullOrEmpty(liveVer) && liveVer != embeddedVer;
                bool newerThanCache    = !string.IsNullOrEmpty(liveVer) && liveVer != cacheVer;

                if (newerThanEmbedded || newerThanCache)
                {
                    // Save to disk cache for offline use
                    try { File.WriteAllText(CachePath(), liveJson); } catch { }
                    finalJson    = liveJson;
                    LoadedVersion = liveVer;
                    UpdateStatus = $"updated  {embeddedVer} → {liveVer}";
                }
                else
                {
                    // Live fetch succeeded but we're already on the right version
                    finalJson    = liveJson;
                    LoadedVersion = liveVer;
                    UpdateStatus = $"up to date  ({liveVer})";
                }
            }
            else if (!string.IsNullOrWhiteSpace(cacheJson))
            {
                // No network — use the disk cache
                finalJson    = cacheJson;
                LoadedVersion = cacheVer;
                UpdateStatus = $"offline — using cached  ({cacheVer})";
            }
            else if (!string.IsNullOrWhiteSpace(embeddedJson))
            {
                // No network, no cache — use the version baked into the exe
                finalJson    = embeddedJson;
                LoadedVersion = embeddedVer;
                UpdateStatus = $"offline — using built-in  ({embeddedVer})";
            }
            else
            {
                throw new Exception("No offsets available (no network, no cache, embedded resource missing).");
            }

            // ── 4. Apply offsets ──────────────────────────────────────────────
            using (JsonDocument doc = JsonDocument.Parse(finalJson))
            {
                JsonElement root = doc.RootElement;
                foreach (var field in typeof(offsets).GetFields(
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
                {
                    if (field.FieldType == typeof(int) && root.TryGetProperty(field.Name, out JsonElement val))
                    {
                        string hex = val.GetString() ?? "";
                        if (hex != "") field.SetValue(null, Convert.ToInt32(hex, 16));
                    }
                }
            }
        }
    }

    // ─────────────────────────────────────────────
    //  MEMORY
    // ─────────────────────────────────────────────
    public class memory
    {
        public static IntPtr handle = IntPtr.Zero;
        public static int pid = 0;

        [DllImport("kernel32.dll")] public static extern IntPtr OpenProcess(int a, bool b, int c);
        [DllImport("kernel32.dll")] public static extern bool ReadProcessMemory(IntPtr h, IntPtr a, byte[] buf, int sz, out int n);
        [DllImport("kernel32.dll")] public static extern bool WriteProcessMemory(IntPtr h, IntPtr a, byte[] buf, int sz, out int n);

        public static bool attach()
        {
            var procs = Process.GetProcessesByName("RobloxPlayerBeta");
            if (procs.Length == 0) return false;
            pid = procs[0].Id;
            handle = OpenProcess(0x1F0FFF, false, pid);
            return handle != IntPtr.Zero;
        }

        public static T read<T>(IntPtr a) where T : struct
        {
            byte[] buf = new byte[Marshal.SizeOf(typeof(T))];
            ReadProcessMemory(handle, a, buf, buf.Length, out _);
            GCHandle g = GCHandle.Alloc(buf, GCHandleType.Pinned);
            T v = Marshal.PtrToStructure<T>(g.AddrOfPinnedObject()); g.Free(); return v;
        }

        public static bool write<T>(IntPtr a, T val) where T : struct
        {
            int sz = Marshal.SizeOf(typeof(T));
            byte[] buf = new byte[sz];
            IntPtr p = Marshal.AllocHGlobal(sz);
            Marshal.StructureToPtr(val, p, false);
            Marshal.Copy(p, buf, 0, sz);
            Marshal.FreeHGlobal(p);
            return WriteProcessMemory(handle, a, buf, sz, out int n) && n == sz;
        }

        public static string readstring(IntPtr a, int max = 200)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < max; i++)
            {
                byte[] b = new byte[1];
                ReadProcessMemory(handle, (IntPtr)((long)a + i), b, 1, out _);
                if (b[0] == 0) break; sb.Append((char)b[0]);
            }
            return sb.ToString();
        }

        // Single-byte helpers needed for bit-flag properties (CanCollide etc.)
        public static byte readbyte(IntPtr a)
        {
            byte[] b = new byte[1];
            ReadProcessMemory(handle, a, b, 1, out _);
            return b[0];
        }
        public static void writebyte(IntPtr a, byte v)
        {
            WriteProcessMemory(handle, a, new byte[] { v }, 1, out _);
        }

        // Resolve the physics Primitive pointer stored inside a BasePart instance.
        // Position, Velocity, and CFrame all live on the Primitive, NOT on the instance itself.
        public static IntPtr prim(IntPtr instance_addr)
        {
            if (instance_addr == IntPtr.Zero) return IntPtr.Zero;
            return read<IntPtr>(instance_addr + offsets.Primitive);
        }

        public static IntPtr getbase()
        {
            foreach (ProcessModule m in Process.GetProcessById(pid).Modules)
                if (m.ModuleName.Equals("RobloxPlayerBeta.exe", StringComparison.OrdinalIgnoreCase))
                    return m.BaseAddress;
            return IntPtr.Zero;
        }
    }

    // ─────────────────────────────────────────────
    //  INSTANCE
    // ─────────────────────────────────────────────
    public class instance
    {
        public IntPtr address;
        public instance(IntPtr a) { address = a; }
        public bool valid() => address != IntPtr.Zero;
        public string name()
        {
            var ptr = memory.read<IntPtr>(address + offsets.Name);
            return ptr != IntPtr.Zero ? memory.readstring(ptr) : "";
        }
        public instance parent() => new instance(memory.read<IntPtr>(address + offsets.Parent));
        public List<instance> getchildren()
        {
            var list = new List<instance>();
            ulong start = memory.read<ulong>(address + offsets.Children);
            ulong end = memory.read<ulong>((IntPtr)start + offsets.ChildrenEnd);
            for (ulong p = memory.read<ulong>((IntPtr)start); p != end; p += 0x10)
                list.Add(new instance((IntPtr)memory.read<ulong>((IntPtr)p)));
            return list;
        }
        public instance find(string n)
        {
            foreach (var c in getchildren())
                if (c.name().Equals(n, StringComparison.OrdinalIgnoreCase)) return c;
            return new instance(IntPtr.Zero);
        }

        // ─────────────────────────────────────────
        //  GAME FUNCTIONS
        // ─────────────────────────────────────────
        public static class rbx
        {
            public static float target_speed   = 16f;
            public static float target_jump    = 50f;
            public static float target_health  = 100f;
            public static bool  loop_enabled   = true;
            public static bool  health_freeze  = false;
            public static bool  anti_afk       = false;
            public static bool  noclip_on      = false;
            public static int   afk_timer      = 0;

            public static IntPtr getdatamodel()
            {
                var b = memory.getbase(); if (b == IntPtr.Zero) return IntPtr.Zero;
                var fake = memory.read<IntPtr>(b + offsets.FakeDataModelPointer);
                return memory.read<IntPtr>(fake + offsets.FakeDataModelToDataModel);
            }

            public static (instance dm, instance wrk, instance plrs, IntPtr lp, instance lpInst) GetCore()
            {
                var dm    = new instance(getdatamodel());
                var wrk   = dm.find("Workspace");
                var plrs  = dm.find("Players");
                var lp    = memory.read<IntPtr>(plrs.address + offsets.LocalPlayer);
                var lpInst= new instance(lp);
                return (dm, wrk, plrs, lp, lpInst);
            }

            public static IntPtr get_humanoid()
            {
                var (dm, wrk, plrs, lp, lpi) = GetCore();
                return wrk.find(lpi.name()).find("Humanoid").address;
            }

            public static IntPtr get_myroot()
            {
                var (dm, wrk, plrs, lp, lpi) = GetCore();
                return wrk.find(lpi.name()).find("HumanoidRootPart").address;
            }

            // Main background loop — handles speed bypass, health freeze, anti-afk, noclip
            public static void loop_thread()
            {
                while (loop_enabled)
                {
                    try
                    {
                        var h = get_humanoid();
                        if (h != IntPtr.Zero)
                        {
                            memory.write(h + offsets.WalkSpeed,    target_speed);
                            memory.write(h + offsets.WalkSpeedCheck, target_speed);
                            memory.write(h + offsets.JumpPower,    target_jump);

                            if (health_freeze)
                            {
                                memory.write(h + offsets.MaxHealth, target_health);
                                memory.write(h + offsets.Health,    target_health);
                            }
                        }

                        if (noclip_on)
                        {
                            var (_, wrk2, _, lp2, lpi2) = GetCore();
                            var charac = wrk2.find(lpi2.name());
                            foreach (var child in charac.getchildren())
                            {
                                if (!child.valid()) continue;
                                // CanCollide is a bit inside a flag byte — must use bitmask, not int write
                                IntPtr flagAddr = child.address + offsets.CanCollide;
                                byte flags = memory.readbyte(flagAddr);
                                // Clear the CanCollide bit (mask 0x8) to disable collision
                                flags = (byte)(flags & ~offsets.CanCollideMask);
                                memory.writebyte(flagAddr, flags);
                            }
                        }

                        if (anti_afk)
                        {
                            afk_timer++;
                            if (afk_timer >= 5500) // ~55 sec at 10ms
                            {
                                var root = get_myroot();
                                IntPtr rootPrim = memory.prim(root);
                                if (rootPrim != IntPtr.Zero)
                                {
                                    // Nudge position slightly via the primitive so it actually registers
                                    float cx = memory.read<float>(rootPrim + offsets.Position);
                                    memory.write(rootPrim + offsets.Position, cx + 0.001f);
                                    Thread.Sleep(50);
                                    memory.write(rootPrim + offsets.Position, cx);
                                }
                                afk_timer = 0;
                            }
                        }
                    }
                    catch { }
                    Thread.Sleep(10);
                }
            }

            // ── Primitive helpers: read/write position & velocity through the physics chain ──

            static (float x, float y, float z) read_pos(IntPtr inst)
            {
                IntPtr p = memory.prim(inst);
                if (p == IntPtr.Zero) return (0, 0, 0);
                return (memory.read<float>(p + offsets.Position),
                        memory.read<float>(p + offsets.Position + 4),
                        memory.read<float>(p + offsets.Position + 8));
            }

            static void write_pos(IntPtr inst, float x, float y, float z)
            {
                IntPtr p = memory.prim(inst);
                if (p == IntPtr.Zero) return;
                memory.write(p + offsets.Position,     x);
                memory.write(p + offsets.Position + 4, y);
                memory.write(p + offsets.Position + 8, z);
            }

            static void write_vel(IntPtr inst, float vx, float vy, float vz)
            {
                IntPtr p = memory.prim(inst);
                if (p == IntPtr.Zero) return;
                memory.write(p + offsets.Velocity,     vx);
                memory.write(p + offsets.Velocity + 4, vy);
                memory.write(p + offsets.Velocity + 8, vz);
            }

            public static (float x, float y, float z) get_my_position()
            {
                return read_pos(get_myroot());
            }

            public static void fling(string username)
            {
                var (dm, wrk, plrs, lp, lpi) = GetCore();
                IntPtr target = IntPtr.Zero;
                foreach (var p in plrs.getchildren())
                    if (p.name().Equals(username, StringComparison.OrdinalIgnoreCase))
                    { target = wrk.find(p.name()).find("HumanoidRootPart").address; break; }
                if (target == IntPtr.Zero) return;
                var myRoot = wrk.find(lpi.name()).find("HumanoidRootPart").address;
                for (int i = 0; i < 500; i++)
                {
                    // Read target position via their primitive
                    var (tx, ty, tz) = read_pos(target);
                    // Write own position via own primitive (teleport to target)
                    write_pos(myRoot, tx, ty, tz);
                    // Massive velocity on own primitive to fling
                    write_vel(myRoot, 9999999f, 9999999f, 9999999f);
                    Thread.Sleep(10);
                }
            }

            public static void teleport_to(string username)
            {
                var (dm, wrk, plrs, lp, lpi) = GetCore();
                IntPtr target = IntPtr.Zero;
                foreach (var p in plrs.getchildren())
                    if (p.name().Equals(username, StringComparison.OrdinalIgnoreCase))
                    { target = wrk.find(p.name()).find("HumanoidRootPart").address; break; }
                if (target == IntPtr.Zero) return;
                var myRoot = wrk.find(lpi.name()).find("HumanoidRootPart").address;
                var (tx, ty, tz) = read_pos(target);
                write_pos(myRoot, tx, ty + 3f, tz);
            }

            public static void teleport_coords(float x, float y, float z)
            {
                var root = get_myroot(); if (root == IntPtr.Zero) return;
                // Write through primitive so the physics engine sees the change
                write_pos(root, x, y, z);
                // Also zero out velocity so there's no carry-over momentum
                write_vel(root, 0f, 0f, 0f);
            }

            public static void set_gravity(float g)
            {
                var (dm, wrk, _, _, _) = GetCore();
                if (!wrk.valid()) return;
                // Write to both the cached property AND the read-only physics copy
                memory.write(wrk.address + offsets.Gravity,         g);
                memory.write(wrk.address + offsets.ReadOnlyGravity, g);
            }

            public static void set_zoom(float dist)
            {
                var (_, wrk, _, _, _) = GetCore();
                var cam = memory.read<IntPtr>(wrk.address + offsets.Camera);
                if (cam != IntPtr.Zero)
                {
                    memory.write(cam + offsets.CameraMaxZoomDistance, dist);
                    memory.write(cam + offsets.CameraMinZoomDistance, 0f);
                }
            }

            public static void unanchor_workspace()
            {
                var (_, wrk, _, _, _) = GetCore();
                void recurse(instance node)
                {
                    foreach (var child in node.getchildren())
                    {
                        if (!child.valid()) continue;
                        IntPtr flagAddr = child.address + offsets.Anchored;
                        byte flags = memory.readbyte(flagAddr);
                        // Clear the Anchored bit (mask 0x2) to unanchor
                        flags = (byte)(flags & ~offsets.AnchoredMask);
                        memory.writebyte(flagAddr, flags);
                        recurse(child);
                    }
                }
                recurse(wrk);
            }

            public static void set_fov(float fov)
            {
                var (dm, wrk, _, _, _) = GetCore();
                var cam = memory.read<IntPtr>(wrk.address + offsets.Camera);
                if (cam != IntPtr.Zero) memory.write(cam + offsets.FOV, fov);
            }

            public static List<string> list_players()
            {
                var names = new List<string>();
                var (_, _, plrs, _, _) = GetCore();
                foreach (var p in plrs.getchildren()) names.Add(p.name());
                return names;
            }

            public static void speed_preset(string preset)
            {
                switch (preset.ToLower())
                {
                    case "slow":    target_speed = 8f;   target_jump = 30f;  break;
                    case "normal":  target_speed = 16f;  target_jump = 50f;  break;
                    case "fast":    target_speed = 60f;  target_jump = 100f; break;
                    case "insane":  target_speed = 250f; target_jump = 300f; break;
                    case "god":     target_speed = 999f; target_jump = 999f; break;
                }
            }
        }

        // ─────────────────────────────────────────
        //  UI HELPERS
        // ─────────────────────────────────────────
        internal class UI
        {
            const int W = 60;

            static void SetColor(ConsoleColor fg, ConsoleColor bg = ConsoleColor.Black)
            { Console.ForegroundColor = fg; Console.BackgroundColor = bg; }

            public static void Banner()
            {
                SetColor(ConsoleColor.DarkCyan);
                Console.WriteLine();
                Console.WriteLine("  ██████╗ ██████╗ ██╗  ██╗    ███████╗██╗  ██╗████████╗");
                Console.WriteLine("  ██╔══██╗██╔══██╗╚██╗██╔╝    ██╔════╝╚██╗██╔╝╚══██╔══╝");
                Console.WriteLine("  ██████╔╝██████╔╝ ╚███╔╝     █████╗   ╚███╔╝    ██║   ");
                Console.WriteLine("  ██╔══██╗██╔══██╗ ██╔██╗     ██╔══╝   ██╔██╗    ██║   ");
                Console.WriteLine("  ██║  ██║██████╔╝██╔╝ ██╗    ███████╗██╔╝ ██╗   ██║   ");
                Console.WriteLine("  ╚═╝  ╚═╝╚═════╝ ╚═╝  ╚═╝   ╚══════╝╚═╝  ╚═╝   ╚═╝   ");
                SetColor(ConsoleColor.DarkGray);
                Console.WriteLine("  external memory tool  //  roblox  //  x64");
                Console.WriteLine();
            }

            public static void Box(string title, ConsoleColor color = ConsoleColor.DarkCyan)
            {
                SetColor(color);
                Console.WriteLine("  ┌" + new string('─', W - 4) + "┐");
                int pad = (W - 4 - title.Length) / 2;
                Console.WriteLine("  │" + new string(' ', pad) + title + new string(' ', W - 4 - pad - title.Length) + "│");
                Console.WriteLine("  ├" + new string('─', W - 4) + "┤");
            }

            public static void BoxEnd(ConsoleColor color = ConsoleColor.DarkCyan)
            {
                SetColor(color);
                Console.WriteLine("  └" + new string('─', W - 4) + "┘");
            }

            public static void Row(string key, string label, ConsoleColor keyColor = ConsoleColor.Cyan, ConsoleColor labelColor = ConsoleColor.White)
            {
                SetColor(ConsoleColor.DarkCyan);
                Console.Write("  │  ");
                SetColor(keyColor);
                Console.Write($"[{key,2}]");
                SetColor(ConsoleColor.DarkGray);
                Console.Write("  ");
                SetColor(labelColor);
                int len = 4 + 2 + 2 + label.Length;
                Console.Write(label.PadRight(W - 4 - 9));
                SetColor(ConsoleColor.DarkCyan);
                Console.WriteLine("│");
            }

            public static void Divider(ConsoleColor color = ConsoleColor.DarkCyan)
            {
                SetColor(color);
                Console.WriteLine("  ├" + new string('─', W - 4) + "┤");
            }

            public static void BlankRow()
            {
                SetColor(ConsoleColor.DarkCyan);
                Console.WriteLine("  │" + new string(' ', W - 4) + "│");
            }

            public static void StatusBar()
            {
                // Row 1 — offset version
                SetColor(ConsoleColor.DarkCyan); Console.Write("  │  ");
                SetColor(ConsoleColor.DarkGray);  Console.Write("offsets  ");
                bool updated = offsets.UpdateStatus.StartsWith("updated");
                SetColor(updated ? ConsoleColor.Green : ConsoleColor.DarkGray);
                string verLine = offsets.UpdateStatus;
                Console.Write(verLine.PadRight(W - 4 - 11));
                SetColor(ConsoleColor.DarkCyan); Console.WriteLine("│");

                // Row 2 — live values
                SetColor(ConsoleColor.DarkCyan); Console.Write("  │  ");
                SetColor(ConsoleColor.DarkGray);  Console.Write("spd ");
                SetColor(ConsoleColor.Yellow);    Console.Write($"{rbx.target_speed,-6:F0}");
                SetColor(ConsoleColor.DarkGray);  Console.Write("  jmp ");
                SetColor(ConsoleColor.Yellow);    Console.Write($"{rbx.target_jump,-6:F0}");
                SetColor(ConsoleColor.DarkGray);  Console.Write("  hp-frz ");
                SetColor(rbx.health_freeze ? ConsoleColor.Green : ConsoleColor.DarkRed);
                Console.Write(rbx.health_freeze ? "ON " : "OFF");
                SetColor(ConsoleColor.DarkGray);  Console.Write("  noclip ");
                SetColor(rbx.noclip_on ? ConsoleColor.Green : ConsoleColor.DarkRed);
                Console.Write(rbx.noclip_on ? "ON " : "OFF");
                SetColor(ConsoleColor.DarkGray);  Console.Write("  afk ");
                SetColor(rbx.anti_afk ? ConsoleColor.Green : ConsoleColor.DarkRed);
                Console.Write(rbx.anti_afk ? "ON " : "OFF");
                SetColor(ConsoleColor.DarkCyan);  Console.WriteLine(new string(' ', Math.Max(0, W - 4 - 59)) + "│");
            }

            public static void Ok(string msg)
            {
                SetColor(ConsoleColor.Green);
                Console.WriteLine($"\n  ✔  {msg}");
                Thread.Sleep(900);
            }

            public static void Err(string msg)
            {
                SetColor(ConsoleColor.DarkRed);
                Console.WriteLine($"\n  ✘  {msg}");
                Thread.Sleep(1100);
            }

            public static string Prompt(string label)
            {
                SetColor(ConsoleColor.DarkGray);
                Console.Write($"\n  ┌─ {label}\n  └▶ ");
                SetColor(ConsoleColor.White);
                return Console.ReadLine() ?? "";
            }

            public static float PromptFloat(string label, float fallback = 0f)
            {
                string raw = Prompt(label);
                return float.TryParse(raw, out float v) ? v : fallback;
            }

            public static void Loading(string msg)
            {
                SetColor(ConsoleColor.DarkGray);
                Console.Write($"  {msg}");
                for (int i = 0; i < 3; i++) { Thread.Sleep(250); Console.Write("."); }
                Console.WriteLine();
            }
        }

        // ─────────────────────────────────────────
        //  ENTRY POINT
        // ─────────────────────────────────────────
        internal class Program
        {
            static async Task Main(string[] args)
            {
                Console.Title = "rbx external  //  x64";
                Console.OutputEncoding = Encoding.UTF8;
                Console.CursorVisible = false;
                Console.Clear();

                UI.Banner();
                UI.Loading("loading offsets");
                await offsets.LoadAsync();
                UI.Loading("attaching to RobloxPlayerBeta");

                if (!memory.attach())
                {
                    UI.Err("could not attach — is Roblox running?");
                    Console.ReadKey();
                    return;
                }

                UI.Ok("attached successfully");
                Thread.Sleep(400);

                rbx.loop_enabled = true;
                _ = Task.Run(() => rbx.loop_thread());

                while (true)
                {
                    Console.Clear();
                    UI.Banner();

                    // Status
                    UI.Box("  STATUS", ConsoleColor.DarkCyan);
                    UI.StatusBar();
                    UI.BoxEnd();

                    Console.WriteLine();

                    // Movement
                    UI.Box("  MOVEMENT", ConsoleColor.DarkBlue);
                    UI.Row(" 1", $"Walk Speed          [{rbx.target_speed:F0}]");
                    UI.Row(" 2", $"Jump Power          [{rbx.target_jump:F0}]");
                    UI.Row(" 3", "Gravity");
                    UI.Row(" 4", "Speed Preset  (slow / normal / fast / insane / god)");
                    UI.Row(" 5", "Hip Height");
                    UI.BoxEnd(ConsoleColor.DarkBlue);

                    Console.WriteLine();

                    // Player
                    UI.Box("  PLAYER", ConsoleColor.DarkMagenta);
                    UI.Row(" 6", "Teleport → Coords");
                    UI.Row(" 7", "Teleport → Username");
                    UI.Row(" 8", "Fling Target");
                    UI.Row(" P", "Show My Position  (reads from primitive)");
                    UI.BoxEnd(ConsoleColor.DarkMagenta);

                    Console.WriteLine();

                    // Toggles
                    UI.Box("  TOGGLES", ConsoleColor.DarkGreen);
                    UI.Row(" 9", $"NoClip              [{(rbx.noclip_on ? "ON " : "OFF")}]", ConsoleColor.Cyan,
                        rbx.noclip_on ? ConsoleColor.Green : ConsoleColor.White);
                    UI.Row("10", $"Health Freeze       [{(rbx.health_freeze ? "ON " : "OFF")}]", ConsoleColor.Cyan,
                        rbx.health_freeze ? ConsoleColor.Green : ConsoleColor.White);
                    UI.Row("11", $"Anti-AFK            [{(rbx.anti_afk ? "ON " : "OFF")}]", ConsoleColor.Cyan,
                        rbx.anti_afk ? ConsoleColor.Green : ConsoleColor.White);
                    UI.BoxEnd(ConsoleColor.DarkGreen);

                    Console.WriteLine();

                    // Misc
                    UI.Box("  MISC", ConsoleColor.DarkYellow);
                    UI.Row("12", "Custom FOV");
                    UI.Row("13", "List Players");
                    UI.Row("14", "Infinite Zoom  (camera max distance)");
                    UI.Row("15", "Unanchor All Parts in Workspace");
                    UI.Row(" 0", "Exit", ConsoleColor.DarkRed, ConsoleColor.DarkRed);
                    UI.BoxEnd(ConsoleColor.DarkYellow);

                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("  option > ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.CursorVisible = true;
                    string cmd = Console.ReadLine() ?? "";
                    Console.CursorVisible = false;

                    try
                    {
                        switch (cmd.Trim())
                        {
                            case "1":
                                rbx.target_speed = UI.PromptFloat("Walk Speed");
                                UI.Ok($"speed set to {rbx.target_speed}");
                                break;

                            case "2":
                                rbx.target_jump = UI.PromptFloat("Jump Power");
                                UI.Ok($"jump power set to {rbx.target_jump}");
                                break;

                            case "3":
                                float g = UI.PromptFloat("Gravity  (default 196.2)");
                                rbx.set_gravity(g);
                                UI.Ok($"gravity set to {g}");
                                break;

                            case "4":
                                string preset = UI.Prompt("Preset  [slow / normal / fast / insane / god]").Trim();
                                rbx.speed_preset(preset);
                                UI.Ok($"preset '{preset}' applied  —  speed {rbx.target_speed}  jump {rbx.target_jump}");
                                break;

                            case "5":
                                float hh = UI.PromptFloat("Hip Height");
                                var hum = rbx.get_humanoid();
                                if (hum != IntPtr.Zero) { memory.write(hum + offsets.HipHeight, hh); UI.Ok($"hip height → {hh}"); }
                                else UI.Err("humanoid not found");
                                break;

                            case "6":
                                float x = UI.PromptFloat("X"); float y = UI.PromptFloat("Y"); float z = UI.PromptFloat("Z");
                                rbx.teleport_coords(x, y, z);
                                UI.Ok($"teleported to {x}, {y}, {z}");
                                break;

                            case "7":
                                string tpUser = UI.Prompt("Username to teleport to");
                                rbx.teleport_to(tpUser);
                                UI.Ok($"teleported to {tpUser}");
                                break;

                            case "8":
                                string fUser = UI.Prompt("Username to fling");
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine($"\n  flinging {fUser}  (5 seconds)...");
                                _ = Task.Run(() => rbx.fling(fUser));
                                Thread.Sleep(400);
                                break;

                            case "9":
                                rbx.noclip_on = !rbx.noclip_on;
                                UI.Ok("noclip " + (rbx.noclip_on ? "enabled" : "disabled"));
                                break;

                            case "10":
                                if (!rbx.health_freeze)
                                {
                                    rbx.target_health = UI.PromptFloat("Health to freeze at  (e.g. 100)", 100f);
                                    rbx.health_freeze = true;
                                    UI.Ok($"health frozen at {rbx.target_health}");
                                }
                                else { rbx.health_freeze = false; UI.Ok("health freeze disabled"); }
                                break;

                            case "11":
                                rbx.anti_afk = !rbx.anti_afk;
                                UI.Ok("anti-afk " + (rbx.anti_afk ? "enabled" : "disabled"));
                                break;

                            case "12":
                                float fov = UI.PromptFloat("FOV  (default 70)");
                                rbx.set_fov(fov);
                                UI.Ok($"fov set to {fov}");
                                break;

                            case "13":
                                var players = rbx.list_players();
                                Console.Clear(); UI.Banner();
                                Console.ForegroundColor = ConsoleColor.DarkCyan;
                                Console.WriteLine("  ┌" + new string('─', 40) + "┐");
                                Console.WriteLine($"  │  Players in server ({players.Count}){"".PadRight(40 - 22 - players.Count.ToString().Length)}│");
                                Console.WriteLine("  ├" + new string('─', 40) + "┤");
                                foreach (var pn in players)
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkCyan; Console.Write("  │  ");
                                    Console.ForegroundColor = ConsoleColor.White; Console.Write(pn.PadRight(38));
                                    Console.ForegroundColor = ConsoleColor.DarkCyan; Console.WriteLine("│");
                                }
                                Console.WriteLine("  └" + new string('─', 40) + "┘");
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write("\n  press any key...");
                                Console.ReadKey(true);
                                break;

                            case "14":
                                float zoom = UI.PromptFloat("Max zoom distance  (e.g. 9999)");
                                rbx.set_zoom(zoom);
                                UI.Ok($"zoom set to {zoom}");
                                break;

                            case "15":
                                rbx.unanchor_workspace();
                                UI.Ok("all parts unanchored");
                                break;

                            case "p":
                            case "P":
                                var (px, py, pz) = rbx.get_my_position();
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write("\n  position  ");
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine($"X {px:F2}   Y {py:F2}   Z {pz:F2}");
                                Thread.Sleep(1800);
                                break;

                            case "0":
                                rbx.loop_enabled = false;
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.WriteLine("\n  goodbye.");
                                Thread.Sleep(700);
                                Environment.Exit(0);
                                break;

                            default:
                                UI.Err("unknown option");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        UI.Err($"error: {ex.Message}");
                    }
                }
            }
        }
    }
}
