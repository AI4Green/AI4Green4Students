import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
  sectionsList: (projectId, experimentId) =>
    `sections?projectId=${projectId}&experimentId=${experimentId}`, // get experiment plan sections of the project and other information based on experimentId
};

export const useSectionsList = (projectId, experimentId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    projectId && experimentId
      ? fetchKeys.sectionsList(projectId, experimentId)
      : null,
    async (url) => {
      const data = await apiFetcher(url);
      return [
        { id: 1, name: "Main", approved: false, comments: 0, sortOrder: 0 },
        {
          id: 2,
          name: "Reaction Scheme",
          approved: false,
          comments: 0,
          sortOrder: 0,
        },
        {
          id: 3,
          name: "Literature Review",
          approved: false,
          comments: 0,
          sortOrder: 0,
        },
        { id: 4, name: "COSH", approved: false, comments: 0, sortOrder: 0 },
        {
          id: 5,
          name: "Safety Data",
          approved: false,
          comments: 0,
          sortOrder: 0,
        },
        {
          id: 6,
          name: "Experimental Procedure",
          approved: false,
          comments: 0,
          sortOrder: 0,
        },
      ];
    },
    { suspense: true }
  );
};
