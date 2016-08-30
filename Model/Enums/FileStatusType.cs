namespace Iata.IS.Model.Enums
{
  public enum FileStatusType
  {
    Received = 1,
    PushedToDestination = 2,
    //SanityCheckFailed = 3,
    SanityCheckError = 4,
    SanityCheckPassedPhaseI = 5,
    SanityCheckPassedPhaseII = 6,
    FormatConversionPassed = 7,// TO BE DISCUSSED WITH PSL BY MANISH AS STATUS IS NOT REQUIRED
    Stored = 8,// TO BE DISCUSSED WITH PSL BY MANISH AS STATUS IS NOT REQUIRED
    AvailableForDownload = 9, //TO BE RENAMED LATER TO FileGenerated
    ReadyForBrdLoader = 16,// TO BE DISCUSSED WITH PSL BY MANISH AS STATUS IS NOT REQUIRED
    CsvGenerationCompleted = 17,// TO BE DISCUSSED WITH PSL BY MANISH AS STATUS IS NOT REQUIRED
    ErrorCorrectable = 18, //TO BE RENAMED LATER TO ErrorInValidation
    ErrorNonCorrectable = 19,//TO BE RENAMED LATER TO ErrorInValidation
    SuccessfullyValidated = 20, //TO BE RENAMED LATER TO SuccessfullyValidated
    ErrorInFileSend=21,
    iiNetRecipientNotFound =22,
    ValidationCompleted = 23,

    #region CMP#608: Load Member Profile - CSV Option

    InProgress = 24,
    Failed = 25,
    Successful = 26

	#endregion
  }
}
