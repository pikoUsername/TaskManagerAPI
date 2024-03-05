﻿namespace TaskManager.Schemas
{
    public class CreateProjectScheme
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty; 
    }

    public class ProjectSelecorScheme { 
        public string? Name { get; set; } 
        public Guid? Id { get; set; }
    }

    public class AddUserProjectScheme
    {
        public Guid UserId { get; set; }
    }
    
    public class UpdateProjectSchema
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Guid? IconId { get; set; }
    }
}
