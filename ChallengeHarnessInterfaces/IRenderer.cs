namespace ChallengeHarnessInterfaces
{
    public interface IRenderer
    {
        MatchRender Render(IMatch match);
        MatchSummary RenderSummary(IMatch match);
    }
}