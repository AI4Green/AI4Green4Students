import { VStack } from "@chakra-ui/react";
import { useField, useFormikContext } from "formik";
import { KetcherEditor } from "./KetcherEditor";
import { ReactionTable } from "./ReactionTable";

export const ReactionScheme = ({ name }) => {
  const [field, meta, helpers] = useField(name);
  const { values } = useFormikContext();

  return (
    <VStack minW="full" spacing={5}>
      <KetcherEditor name={`${name}.reactionSketch`} />
      {values[name]?.reactionSketch?.data && (
        <ReactionTable
          name={`${name}.reactionTable`}
          ketcherData={values[name]?.reactionSketch?.data}
        />
      )}
    </VStack>
  );
};
