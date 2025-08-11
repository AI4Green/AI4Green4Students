import { useUser } from "contexts";
import { SITE_ROLES } from "constants";

/**
 * Simple hook to check if the current user is an instructor.
 * TODO: Since instructor can only access their projects, this may need updating to check if the user is an instructor for the given project.
 * @returns boolean
 */
export const useIsInstructor = () => {
  const { user } = useUser();
  return user?.roles?.includes(SITE_ROLES.Instructor);
};
