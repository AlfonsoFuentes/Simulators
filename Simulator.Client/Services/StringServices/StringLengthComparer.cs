namespace Simulator.Client.Services.StringServices
{
    public class StringLengthComparer : IComparer<string>
    {
        public static readonly StringLengthComparer Instance = new StringLengthComparer();

        public int Compare(string? x, string? y)
        {
            if (x is null)
            {
                return y is null ? 0 : -1;
            }

            if (y is null)
            {
                return 1;
            }

            return x.Length.CompareTo(y.Length);
        }
    }
}
