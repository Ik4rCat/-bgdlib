// bgdlib/Models/FlatComment.cs
// A flattened Reddit-style comment node produced by PostDetailViewModel.
// Each node stores its depth, rail colour, vote state, etc.

namespace bgdlib.Models;

public class FlatComment : Microsoft.Maui.Controls.BindableObject
{
    // ── Source data ──────────────────────────────────────────────────
    public int    Id         { get; set; }
    public string UserName   { get; set; } = "";
    public string AvatarLetter => UserName.Length > 0 ? UserName[0].ToString().ToUpper() : "?";
    public bool   IsOp       { get; set; }          // OP badge
    public string TimeLabel  { get; set; } = "";
    public string Body       { get; set; } = "";
    public int    BaseVotes  { get; set; }

    // ── Threading ────────────────────────────────────────────────────
    public int    Depth      { get; set; }          // 0 = root
    public double IndentPx   => Depth * 14.0;
    public bool   ShowRail   => Depth > 0;
    public Color  RailColor  { get; set; } = Colors.Transparent;
    public int    HiddenCount { get; set; }         // descendants when collapsed
    public string HiddenLabel => HiddenCount == 1 ? "1 ответ" : $"{HiddenCount} ответов";

    // ── Vote state (bound two-way via VM) ─────────────────────────────
    private int _vote = 0;           // -1 | 0 | 1
    public  int VoteDelta
    {
        get => _vote;
        set { _vote = value; OnPropertyChanged(); OnPropertyChanged(nameof(Score));
              OnPropertyChanged(nameof(UpColor)); OnPropertyChanged(nameof(DownColor));
              OnPropertyChanged(nameof(ScoreColor)); }
    }
    public int    Score      => BaseVotes + VoteDelta;
    public Color  UpColor    => VoteDelta ==  1 ? Color.FromArgb("#E53935") : Color.FromArgb("#7C7C7C");
    public Color  DownColor  => VoteDelta == -1 ? Color.FromArgb("#5A8DEE") : Color.FromArgb("#7C7C7C");
    public Color  ScoreColor => VoteDelta ==  1 ? Color.FromArgb("#E53935")
                              : VoteDelta == -1 ? Color.FromArgb("#5A8DEE")
                              : Color.FromArgb("#B0B0B0");

    // ── Collapse state ────────────────────────────────────────────────
    private bool _collapsed;
    public bool Collapsed
    {
        get => _collapsed;
        set { _collapsed = value; OnPropertyChanged(); OnPropertyChanged(nameof(BodyVisible));
              OnPropertyChanged(nameof(CollapseGlyph)); }
    }
    public bool   BodyVisible  => !_collapsed;
    public string CollapseGlyph => _collapsed ? "\ue145" : "\ue15b";   // add / remove

    // ── Avatar colour (deterministic from username) ───────────────────
    public Color AvatarColor
    {
        get
        {
            float hue = (UserName.Aggregate(0, (a, c) => a + c) * 47 % 360) / 360f;
            return Color.FromHsla(hue, 0.55, 0.48);
        }
    }
}
