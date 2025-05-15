using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core
{

    [DataContract]
    public enum ValidationSeverityEnum
    {
        [EnumMember]
        Information = 1,

        [EnumMember]
        Warning = 2,

        [EnumMember]
        Error = 3
    }

    [DataContract]
    public class ValidationResult
    {
        [DataMember]
        public string StealthMessage { get; set; }

        [DataMember]
        public string ValidationMessage { get; set; }

        [DataMember]
        public string ExceptionInformation { get; set; }

        [DataMember]
        public int ValidationCode { get; set; }

        [DataMember]
        public string ValidationTag { get; set; }

        [DataMember]
        public bool IsValid { get; set; }

        [DataMember]
        public ValidationSeverityEnum ValidationSeverity { get; set; }

        public ValidationResult()
        {
            IsValid = false; // backwards compatability
            ValidationSeverity = ValidationSeverityEnum.Error; // backwards compatability
        }

        public void AssignExceptionInformation(Exception exception)
        {
            var sb = new StringBuilder();

            var exceptionBeingInspected = exception;
            while (exceptionBeingInspected != null)
            {
                sb.AppendLine($"Message: {exceptionBeingInspected.Message}");
                sb.AppendLine($"ExceptionType: {exceptionBeingInspected.GetType()}");
                sb.AppendLine($"HelpLink: {exceptionBeingInspected.HelpLink}");
                sb.AppendLine($"Source: {exceptionBeingInspected.Source}");
                sb.AppendLine($"TargetSite: {exceptionBeingInspected.TargetSite}");
                sb.AppendLine($"StackTrace: {exceptionBeingInspected.StackTrace}");
                exceptionBeingInspected = exceptionBeingInspected.InnerException;
            }

            ExceptionInformation = sb.ToString();
        }

        public ValidationResult(string message, Enum enumCategory = null)
        {
            ValidationMessage = message;
            ValidationSeverity = ValidationSeverityEnum.Error; // backwards compatability
            if (enumCategory == null) return;
            ValidationTag = GetEnumName(enumCategory);
            ValidationCode = enumCategory.GetHashCode();
        }

        public static string GetEnumName(Enum theEnum)
        {
            if (theEnum == null) return string.Empty;

            var sb = new StringBuilder();
            var enumType = theEnum.GetType();

            if (enumType.DeclaringType != null) sb.Append(enumType.DeclaringType.Name + ".");

            sb.Append(enumType.Name + ".");
            sb.Append(theEnum);

            return sb.ToString();
        }

    }
}
