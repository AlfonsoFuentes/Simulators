namespace Simulator.Shared.Commons.BaseResponses
{
    public abstract class ValueObject 
    {
        public string Name { get; set; } = string.Empty;
    


        protected static bool EqualOperator(BaseResponse left, BaseResponse right)
        {
            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
            {
                return false;
            }
            return ReferenceEquals(left, right) || left!.Equals(right!);
        }

        protected static bool NotEqualOperator(BaseResponse left, BaseResponse right)
        {
            return !EqualOperator(left, right);
        }

        protected virtual IEnumerable<object> GetEqualityComponents()
        {
      
            yield return Name;

        }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            var other = (ValueObject)obj;

            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
        }


    }
}
