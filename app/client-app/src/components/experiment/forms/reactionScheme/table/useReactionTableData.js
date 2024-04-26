import { useField } from "formik";
import { useEffect, useMemo, useState } from "react";

export const useReactionTable = (name, ketcherData) => {
  const [field, meta, helpers] = useField(name);
  const hasExistingTableData = field.value?.tableData?.length >= 1;

  const { initialTableData } = useInitialReactionTableData(ketcherData);
  const initialData = hasExistingTableData
    ? field.value.tableData
    : initialTableData;

  const [tableData, setTableData] = useState(initialData);

  useEffect(() => {
    helpers.setValue({ ...field.value, tableData });
  }, [tableData]);

  return { tableData, setTableData };
};

const useInitialReactionTableData = (reactionData) => {
  const initialTableData = useMemo(
    () =>
      reactionData?.map((data) => ({
        substanceType: data?.substanceType,
        substancesUsed: data?.name,
        molWeight: data?.molecularWeight,
        density: data?.density,
        hazards: data?.hazards,
        limiting: false,
      })),
    [reactionData]
  );

  return { initialTableData: initialTableData ?? [] };
};
