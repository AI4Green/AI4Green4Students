import { VStack } from "@chakra-ui/react";
import { useField } from "formik";
import { KetcherEditor } from "./KetcherEditor";
import { ReactionTable } from "./ReactionTable";

/**
 *
 * @param {*} param0
 * - name: formik field name
 * - isDisabled: boolean (whether the component is disabled or not).
 *  - is passed to KetcherEditor and ReactionTable to set the disabled state of the components
 * @returns
 */
export const ReactionScheme = ({ name, isDisabled }) => {
  const [field, meta, helpers] = useField(name);

  return (
    <VStack minW="full" spacing={5}>
      <KetcherEditor
        parentName={name}
        name={`${name}.reactionSketch`}
        {...{ isDisabled }}
      />
      {field.value?.reactionSketch?.data && (
        <ReactionTable
          name={`${name}.reactionTable`}
          ketcherData={field.value?.reactionSketch?.data}
          {...{ isDisabled }}
        />
      )}
    </VStack>
  );
};
