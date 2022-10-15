using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Field)]
    public class FileSizeAttribute : ValidationAttribute, IClientModelValidator
    {
        public long FileSize { get; set; }
        const string ErrorMess = "The file is too large!";
        public FileSizeAttribute(long fileSize)
        {
            FileSize = fileSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            else if (value is IFormFile formFile)
            {
                if (formFile.Length < FileSize)
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult(ErrorMess, new List<string>() { validationContext.MemberName });
            }

            throw new NotImplementedException($"Validation is not supported for this type: {value.GetType()}");
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val1", "true");
            context.Attributes.Add("data-val1-filesize", ErrorMess);
            context.Attributes.Add("data-val1-filesize-size", FileSize.ToString());
        }
    }
}