import { useField } from "formik";
import { ReactionScheme } from "../ReactionScheme";
import {
  VStack,
  Tabs,
  TabList,
  TabPanels,
  Tab,
  TabPanel,
  Text,
} from "@chakra-ui/react";

/**
 *
 * @param {*} param0
 * - name: formik field name
 * - isDisabled: boolean (whether the component is disabled or not).
 *  - is passed to KetcherEditor and ReactionTable to set the disabled state of the components
 * @returns
 */

export const MultiReactionScheme = ({ name, isDisabled }) => {
  const [field, meta, helpers] = useField(name);
  return (
    <VStack spacing={4}>
      {field?.value?.length > 0 ? (
        <Tabs
          isLazy
          marginBottom={10}
          p={5}
          border="1px"
          borderRadius="12"
          borderColor="gray.200"
        >
          <TabList>
            {field.value.map((val, index) => (
              <Tab key={index}>Reaction Scheme {index + 1}</Tab>
            ))}
          </TabList>
          <TabPanels>
            {field.value.map((val, index) => (
              <TabPanel key={index}>
                <ReactionScheme
                  name={`${name}[${index}]`}
                  label="Reaction Scheme"
                  isDisabled={isDisabled}
                />
              </TabPanel>
            ))}
          </TabPanels>
        </Tabs>
      ) : (
        <Text pt={5} pb={5}>
          No reaction schemes have been added to this report
        </Text>
      )}
    </VStack>
  );
};
