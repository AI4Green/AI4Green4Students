import { useField } from "formik";
import { ProductYieldTable } from "../ProductYieldTable";
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
 * Formik field for multiple product yield tables.
 * expects an object array with the following properties:
 * - source: an object with properties.
 *  - type: Type of the source. E.g. Plan or Lab Note
 *  - name: Name/title if available. E.g. plan name or reaction name
 *  - id: Id of the source. E.g. Plan or Note id
 * - data: an array of product yield data.
 */
export const MultiProductYieldTable = ({ name, isDisabled }) => {
  const [field, meta, helpers] = useField(name);

  return (
    <VStack spacing={4} w="full">
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
            {field.value.map((val) => (
              <Tab key={`${val.source?.type}.${val.source?.id}`}>
                Yield Table - {val.source?.name}
              </Tab>
            ))}
          </TabList>
          <TabPanels>
            {field.value.map((val, index) => (
              <TabPanel key={`${val.source?.type}.${val.source?.id}`}>
                <ProductYieldTable
                  name={`${name}[${index}].data`}
                  label={`Yield Table - ${val.source?.name}`}
                  isDisabled={isDisabled}
                />
              </TabPanel>
            ))}
          </TabPanels>
        </Tabs>
      ) : (
        <Text pt={5} pb={5}>
          No yield table available.
        </Text>
      )}
    </VStack>
  );
};
