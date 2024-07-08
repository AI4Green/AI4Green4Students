import { VStack } from "@chakra-ui/react";
import { useField } from "formik";
import { KetcherEditor } from "./KetcherEditor";
import { ReactionTable } from "./table/ReactionTable";

/**
 * Formik field for reaction scheme.
 * Props:
 * - name: formik field name
 * - isDisabled: boolean (whether the component is disabled or not).
 *  - is passed to KetcherEditor and ReactionTable to set the disabled state of the components
 *
 * field value structure would be an object with the following properties:
 * - reactionSketch: reaction sketch object
 * - reactionTable: array (data for the reaction table)
 */
export const ReactionScheme = ({ name, isDisabled }) => {
  const [field, meta, helpers] = useField(name);

  return (
    <VStack minW="full" spacing={5}>
      <KetcherEditor
        parentName={name}
        name={`${name}.reactionSketch`}
        isDisabled={isDisabled}
      />
      {field.value?.reactionSketch?.data && (
        <ReactionTable
          name={`${name}.reactionTable`}
          ketcherData={field.value?.reactionSketch?.data}
          isDisabled={isDisabled}
        />
      )}
    </VStack>
  );
};
