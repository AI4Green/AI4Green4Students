import { useSortingAndFiltering } from "./useSortingAndFiltering";

export const useProjectSortingAndFiltering = (data, storageKey) =>
  useSortingAndFiltering(data, "projectCode", {
    initialSort: {
      key: "projectCode",
    },
    sorters: {
      projectCode: {
        sorter: (asc) => (a, b) =>
          asc ? a.localeCompare(b) : b.localeCompare(a),
      },
    },
    storageKey,
  });
