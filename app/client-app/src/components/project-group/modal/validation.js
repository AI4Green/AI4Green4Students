import { object, string } from "yup";

export const validationSchema = (project) =>
  object().shape({
    name: string()
      .required("Project Group name required")
      .test(
        "unique-project-group-name",
        "Project Group name already exists for the Project",
        (value, { parent }) => {
          const { projectId } = parent; // get the projectId from parent object
          if (!projectId || !value) return false; //  fails if projectId or value is not present
          return (
            project && //  fails if project is not found
            !project.projectGroups.some(
              (group) => group.name.toUpperCase() === value.toUpperCase() // also fails if projectGroup  already exists
            )
          );
        }
      ),
  });
