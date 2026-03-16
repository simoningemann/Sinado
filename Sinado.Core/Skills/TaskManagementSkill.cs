using Microsoft.Agents.AI.Skills;

public class TaskManagementSkill {
    [SkillDefinition("Creates a new project sprint with start and end dates")]
    public async Task<string> CreateSprint(DateTime start, DateTime end, string name) {
        // Logic to save to Azure SQL goes here
        return $"Sprint '{name}' created for {start.ToShortDateString()} to {end.ToShortDateString()}.";
    }
}