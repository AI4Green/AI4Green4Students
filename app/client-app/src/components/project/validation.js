import { date, object, string } from "yup";

export const projectValidationSchema = (projects) =>
  object().shape({
    name: string()
      .notOneOf(
        // fails if project name already exists
        projects.map((project) => project.name),
        "Project name already exist"
      )
      .required("Project name required"),
    startDate: date().required("Start date required"),
    planningDeadline: date().required("Planning deadline date required"),
    experimentDeadline: date().required("Experiment deadline date required"),
  });

export const projectGroupNameValidationSchema = (project) =>
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
