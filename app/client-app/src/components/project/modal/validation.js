import { date, object, string } from "yup";

export const validationSchema = (projects) =>
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
