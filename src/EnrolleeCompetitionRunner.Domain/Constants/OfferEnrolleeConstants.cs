namespace EnrolleeCompetitionRunner.Domain.Constants;
public static class OfferEnrolleeConstants
{
    public const int MaxPriority = 5;

    // {"1":"заява надійшла з сайту","2":"затримано","3":"скасовано вступником","4":"скасовано (втрата пріор.)","5":"зареєстровано","6":"допущено","7":"відмова","8":"скасовано ЗО","9":"рекомендовано (бюджет)","10":"відхилено (бюджет)","11":"допущено (контракт, за ріш. ПК)","12":"рекомендовано (контракт)","13":"відхилено (контракт)","14":"до наказу","15":"відраховано","16":"скасовано (зарах. на бюджет)"}
    public static class Statuses
    {
        public const int CameFromSite = 1;                          // Заява надійшла з сайту.
        public const int Delayed = 2;                               // Затримано.
        public const int CancelledByEnrollee = 4;                   // Скасовано вступником.
        public const int CancelledWithPriorityLoss = 4;             // Скасовано (втрата пріор.).
        public const int Registered = 5;                            // Зареєстровано.
        public const int Admitted = 6;                              // Допущено.
        public const int Refusal = 7;                               // Відмова.
        public const int CancelledByUniversity = 8;                 // Скасовано ЗО.
        public const int RecommendedBudget = 9;                     // Рекомендовано (бюджет).
        public const int RejectedBudget = 10;                       // Відхилено (бюджет).
        public const int RecommendedContractByCommission = 11;      // Допущено (контракт, за ріш. ПК).
        public const int RecommendedContract = 12;                  // Рекомендовано (контракт).
        public const int RejectedContract = 13;                     // Відхилено (контракт).
        public const int IncludedToOrder = 14;                      // До наказу.
        public const int Expelled = 15;                             // Відраховано.
        public const int CancelledWithEnrollmentOnBudget = 16;      // Скасовано (зарах. на бюджет).
    }
}
