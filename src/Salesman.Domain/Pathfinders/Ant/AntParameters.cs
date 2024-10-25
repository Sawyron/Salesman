namespace Salesman.Domain.Pathfinders.Ant;

public record AntParameters(
    double Alpha,
    double Beta,
    double Q,
    double P,
    double InitialPheromone,
    int IterationsWithoutImprovementsThreshold);
