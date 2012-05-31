
namespace HardCard.Scoring.Simulator
{
    internal interface ILineProcessStrategy
    {
        TagInfo Process(string line);
    }
}
