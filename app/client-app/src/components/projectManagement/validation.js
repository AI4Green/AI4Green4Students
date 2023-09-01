import { object, string, array, number } from "yup";

export const projectNameValidationSchema = (projects) =>
  object().shape({
    name: string()
      .notOneOf(
        // fails if project name already exists
        projects.map((project) => project.name),
        "Project name already exist"
      )
      .required("Project name required"),
  });

export const projectGroupNameValidationSchema = (
  projects // projects is an array of projects, contains an array of projectGroups
) =>
  object().shape({
    projectId: array()
      .required("Project required")
      .of(
        number().oneOf(
          projects.map((project) => project.id),
          "Invalid Project"
        )
      ),
    name: string()
      .required("Project Group name required")
      .test(
        "unique-project-group-name",
        "Project Group name already exists for the Project",
        (value, { parent }) => {
          const { projectId } = parent; // get the projectId from parent object
          if (!projectId || !value) return false; //  fails if projectId or value is not present

          const project = projects.find((p) => p.id === projectId[0]);
          return (
            project && //  fails if project is not found
            !project.projectGroups.some(
              (group) => group.name.toUpperCase() === value.toUpperCase() // also fails if projectGroup  already exists
            )
          );
        }
      ),
  });
