namespace SSOService.Models.Constants
{
    public static class ValidationConstants
    {
        public const string EmptyRequiredFieldResponse = "{0} is required";
        public const string InvalidFieldResponse = "{0} is an invalid {1}";
        public const string InvalidFieldFormatResponse = "{0} is not in the correct format";
        public const string FieldNotFound = "{0} not Found";
        public const string EntityChangedByAnotherUser = "{0} was updated by another user";
        public const string EntityAlreadyExist = "{0} with {1} {2} already exist";
        public const string NotAvailable = "NA";
    }
}
