import { useUser } from "contexts/User";
import { PROJECTMANAGEMENT_PERMISSIONS } from "constants/site-permissions";
import { EXPERIMENTS_PERMISSIONS } from "constants/site-permissions";

/**
 * Simple hook to check if the current user is an instructor.
 * True if the user has both ViewAllProjects and ViewAllExperiments permission.
 * @returns boolean
 */
export const useIsInstructor = () => {
  const { ViewAllProjects } = PROJECTMANAGEMENT_PERMISSIONS;
  const { ViewAllExperiments } = EXPERIMENTS_PERMISSIONS;
  const { user } = useUser();
  return [ViewAllProjects, ViewAllExperiments].every((permission) =>
    user?.permissions?.includes(permission)
  );
};
