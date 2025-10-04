namespace elastic_orchestrator_service.src.Models
{
    public class Player
    {
        public int IdPlayer { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime? Birthday { get; set; }
        public string Nationality { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public List<PhysicalMetric> PhysicalMetrics { get; set; } = new();
    }

    public class PhysicalMetric
    {
        public int? VerticalJump { get; set; }
        public int? FatPercentage { get; set; }
        public int? BenchPressWeight { get; set; }
        public int? SquatWeight { get; set; }
        public int? SprintSpeed { get; set; }
        public int? Weight { get; set; }
        public int? Height { get; set; }
        public int? Wingspan { get; set; }
        public DateTime? DateOfMeasurement { get; set; }
    }

    public class UpdatePlayerRequest
    {
        public Player Player { get; set; } = new();
    }

    public class ScoutingUpdatePlayerCommand
    {
        public int IdPlayer { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Birthday { get; set; } // DateOnly as string in format "YYYY-MM-DD"
        public int? Weight { get; set; }
        public int? Height { get; set; }
        public int? IdNationality { get; set; }
        public int? IdPosition { get; set; }
    }

    public class SagaTransaction
    {
        public Guid TransactionId { get; set; }
        public string Status { get; set; } = string.Empty;
        public Player Player { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public List<SagaStep> Steps { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public Dictionary<string, string> OriginalStates { get; set; } = new();
    }

    public class SagaStep
    {
        public string StepName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? ExecutedAt { get; set; }
        public string? ErrorMessage { get; set; }
        public bool IsCompensated { get; set; }
        public DateTime? CompensatedAt { get; set; }
    }

    public static class SagaStatuses
    {
        public const string Started = "Started";
        public const string InProgress = "InProgress";
        public const string Completed = "Completed";
        public const string Failed = "Failed";
        public const string Compensating = "Compensating";
        public const string Compensated = "Compensated";
    }

    public static class StepStatuses
    {
        public const string Pending = "Pending";
        public const string InProgress = "InProgress";
        public const string Completed = "Completed";
        public const string Failed = "Failed";
        public const string Compensated = "Compensated";
    }
}