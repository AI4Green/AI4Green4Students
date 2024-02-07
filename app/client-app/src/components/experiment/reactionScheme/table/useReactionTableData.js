import { useState, useEffect, useMemo } from "react";
import { useField } from "formik";

export const useReactionTable = (name, ketcherData) => {
  const [field, meta, helpers] = useField(name);
  const hasExistingTableData = field.value?.tableData?.length >= 1;

  const { initialTableData } = useInitialReactionTableData(ketcherData);
  const initialData = hasExistingTableData
    ? field.value.tableData
    : initialTableData;

  const [tableData, setTableData] = useState(initialData);
  const [massUnit, setMassUnit] = useState(field.value?.massUnit || "cm3");

  useEffect(() => {
    helpers.setValue({ ...field.value, tableData });
  }, [tableData]);

  useEffect(() => {
    helpers.setValue({ ...field.value, massUnit });
  }, [massUnit]);

  return { tableData, setTableData, massUnit, setMassUnit };
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
