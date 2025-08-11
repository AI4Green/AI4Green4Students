import { useIsInstructor } from ".";

/**
 * Hook to check if the user can manage projects.
 * Currently, simply checks if the user is an instructor but in the future can be extended to check for specific permissions.
 * @returns {boolean}
 */
export const useCanManageProject = () => {
  const isInstructor = useIsInstructor();
  return isInstructor;
};
