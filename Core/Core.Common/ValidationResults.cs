using System.Runtime.Serialization;
using System.Text;

namespace Core.Common
{
    [DataContract]
    [KnownType(typeof(ValidationResult))]
    public class ValidationResults
    {
        private BlockingCollectionAdapter<ValidationResult> _ValidationResults;

        public ValidationResults()
        {
            _ValidationResults = new BlockingCollectionAdapter<ValidationResult>();
        }

        [DataMember]
        public BlockingCollectionAdapter<ValidationResult> ValidationResultList
        {
            get { return _ValidationResults; }
            set { _ValidationResults = value; }
        }

        public bool Exists(Enum enumCategory)
        {
            if (_ValidationResults == null || _ValidationResults.Count == 0) return false;

            var enumValue = enumCategory.GetHashCode();

            var count = _ValidationResults.Count(x => x.ValidationCode == enumValue);
            return count > 0;
        }

        public bool Exists(Enum[] enumCategoryList)
        {
            if (_ValidationResults == null || _ValidationResults.Count == 0 || enumCategoryList == null || !enumCategoryList.Any()) return false;

            var enumValueList = enumCategoryList.Select(x => x.GetHashCode()).ToArray();

            var count = _ValidationResults.Count(x => enumValueList.Contains(x.ValidationCode));
            return count > 0;
        }

        public string CombinedMessages
        {
            get
            {
                if (IsValid) return null;
                var sb = new StringBuilder();
                foreach (var validationResult in _ValidationResults)
                {
                    sb.AppendLine(validationResult.ValidationMessage);
                    if (string.IsNullOrWhiteSpace(validationResult.ExceptionInformation)) continue;
                    sb.AppendLine(validationResult.ExceptionInformation);
                }
                return sb.ToString();
            }
        }

        public string FirstMessage => IsValid ? null : _ValidationResults.First().ValidationMessage;

        public int FirstCode => IsValid ? 0 : _ValidationResults.First().ValidationCode;

        public string FirstTag => IsValid ? null : _ValidationResults.First().ValidationTag;

        public void AddResult(string message, Enum enumCategory = null)
        {
            _ValidationResults.Add(new ValidationResult(message, enumCategory));
        }

        public void AddResult(ValidationResult validationResult)
        {
            _ValidationResults.Add(validationResult);
        }

        public void AddAllResults(IEnumerable<ValidationResult> validationResults)
        {
            foreach (var validationResult in validationResults) AddResult(validationResult);
        }

        public void AddAllResults(ValidationResults validationResults)
        {
            foreach (var validationResult in validationResults._ValidationResults) AddResult(validationResult);
        }

        public bool IsValid
        {
            get { return _ValidationResults.All(x => x.IsValid); }
        }
    }
}
