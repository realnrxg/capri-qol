using System;
using System.Drawing;
using System.Windows.Forms;

namespace CapriDesktop
{
    internal sealed partial class Companion
    {
        private double charmAt, charmSaved, nextPrank, prankEnd;
        private MischiefOverlay prank;
        private Point prankOrigin, prankMoved;
        private bool nudged;

        internal int Charm { get { return CharmValue(preferences.CharmSeconds); } }
        internal bool Mischievous { get { return preferences.Mischief; } }
        private static int CharmValue(double seconds) { return (int)Math.Min(100, Math.Max(0, double.IsNaN(seconds) ? 0 : seconds) / 360); }

        internal void SetMischief(bool on)
        {
            preferences.Mischief = on;
            if (!on) EndPrank();
            SavePreferences();
            mainframe.ShowCharm();
        }

        private void CheckPlayfulnessFeatures()
        {
            if (CharmValue(-1) != 0 || CharmValue(double.NaN) != 0 || CharmValue(360) != 1 || CharmValue(999999) != 100) throw new Exception("Charm bounds failed.");
            bool enabled = preferences.Mischief;
            preferences.Mischief = false;
            StartPrank();
            if (prank != null || nudged) throw new Exception("Mischief ran while disabled.");
            preferences.Mischief = enabled;
        }

        private void CheckPlayfulness()
        {
            mainframe.ShowCharm();
            double now = clock.Elapsed.TotalSeconds;
            double elapsed = Math.Max(0, Math.Min(1, now - charmAt));
            charmAt = now;
            if (!hidden && Native.IdleMilliseconds() < 60000)
                preferences.CharmSeconds = Math.Min(36000, Math.Max(0, double.IsNaN(preferences.CharmSeconds) ? 0 : preferences.CharmSeconds) + elapsed);
            if (now - charmSaved >= 60) { charmSaved = now; SavePreferences(); }
            if (preferences.Mischief && Computer.Escape())
            {
                preferences.Mischief = false;
                EndPrank();
                SavePreferences();
                return;
            }
            if (prank != null && (!preferences.Mischief || now >= prankEnd || hidden || job != null)) EndPrank();
            if (nextPrank == 0) nextPrank = now + 120;
            if (preferences.Mischief && now >= nextPrank && !hidden && !chatting && !dragging && !pressed && job == null && talkState == TalkState.Off && !mainframe.Visible && Native.IdleMilliseconds() < 60000)
                StartPrank();
        }

        internal void StartPrank()
        {
            if (!preferences.Mischief || prank != null || job != null || dragging || pressed) return;
            nextPrank = clock.Elapsed.TotalSeconds + 120 + random.Next(180);
            prankEnd = clock.Elapsed.TotalSeconds + 1.2;
            int kind = random.Next(3);
            prank = new MischiefOverlay(Screen.FromPoint(Cursor.Position).Bounds, kind);
            prank.Show();
            if (kind == 1 && Control.MouseButtons == MouseButtons.None)
            {
                prankOrigin = Cursor.Position;
                Rectangle screen = Screen.FromPoint(prankOrigin).Bounds;
                prankMoved = new Point(Math.Min(screen.Right - 1, prankOrigin.X + 36), prankOrigin.Y);
                Cursor.Position = prankMoved;
                nudged = true;
            }
        }

        private void EndPrank()
        {
            if (prank != null) { prank.Close(); prank.Dispose(); prank = null; }
            if (nudged && Cursor.Position == prankMoved) Cursor.Position = prankOrigin;
            nudged = false;
        }

    }

    internal sealed class MischiefOverlay : Form
    {
        private readonly int kind;
        internal MischiefOverlay(Rectangle bounds, int effect)
        {
            kind = effect;
            Bounds = bounds; FormBorderStyle = FormBorderStyle.None; ShowInTaskbar = false; TopMost = true;
            BackColor = Color.Magenta; TransparencyKey = Color.Magenta;
            DoubleBuffered = true;
        }
        protected override bool ShowWithoutActivation { get { return true; } }
        protected override CreateParams CreateParams { get { CreateParams p = base.CreateParams; p.ExStyle |= 0x08000000 | 0x20 | 0x80; return p; } }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Pen pen = new Pen(Color.Cyan, 3))
                for (int i = 0; i < 6; i++) { int y = Height / 3 + i * 17; e.Graphics.DrawLine(pen, Width / 4 + i*13, y, Width * 3 / 4, y); }
            using (Font font = new Font("Segoe UI", 20, FontStyle.Bold))
            using (Brush brush = new SolidBrush(Color.Cyan))
                e.Graphics.DrawString(kind == 1 ? "Borrowed your cursor. — Capri" : "YOU'VE BEEN HACKED\nJust kidding. — Capri", font, brush, Width / 4, Height / 2);
        }
    }
}
