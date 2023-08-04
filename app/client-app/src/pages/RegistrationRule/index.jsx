import {
  TableContainer,
  Table,
  Thead,
  Tr,
  Th,
  Tbody,
  Button,
  Text,
  VStack,
  Heading,
  Center,
  Td,
  HStack,
} from "@chakra-ui/react";
import { FaPlus, FaLink, FaTrash } from "react-icons/fa";
import { useSortingAndFiltering } from "helpers/hooks/useSortingAndFiltering";
import { useDisclosure } from "@chakra-ui/react";
import { SortingAndFilteringPanel } from "components/SortingAndFilteringPanel";
import { ModalCreateOrEditRegistrationRule as ModalNewRegistratonRule } from "./modal/ModalCreateOrEditRegistrationRule";
import { ModalCreateOrEditRegistrationRule as ModalEditRegistratonRule } from "./modal/ModalCreateOrEditRegistrationRule";
import { ModalDeleteRegistrationRule } from "./modal/ModalDeleteRegistrationRule";
import { useRegistrationRulesList } from "api/registrationRules";

// Extending useSortingAndFiltering hook
const useUserSortingAndFiltering = (data, sortingStorageKey, storageKey) =>
  useSortingAndFiltering(data, sortingStorageKey, {
    initialSort: { key: sortingStorageKey },
    sorters: {
      // sorters key name should match with 'sortingStorageKey'
      [sortingStorageKey]: {
        sorter: (asc) => (a, b) =>
          asc ? a.localeCompare(b) : b.localeCompare(a),
      },
    },
    storageKey,
  });

export const RegistrationRule = () => {
  const sortingStorageKey = "value";
  const storageKey = "registrationRules";

  const { data: registrationRules } = useRegistrationRulesList();

  const { sorting, onSort, filter, setFilter, outputList } =
    useUserSortingAndFiltering(
      registrationRules,
      sortingStorageKey,
      storageKey
    );

  const NewRegistrationRuleState = useDisclosure();

  const NewButton = () => (
    <Button
      onClick={NewRegistrationRuleState.onOpen}
      colorScheme="green"
      leftIcon={<FaPlus />}
    >
      <Text
        textTransform="uppercase"
        fontWeight={700}
        fontSize="sm"
        letterSpacing={1.1}
      >
        New
      </Text>
    </Button>
  );

  const RegRulesTable = ({ tableTitle, tableBody }) => (
    <VStack align="stretch" px={4}>
      <Heading as="h2" size="md" fontWeight="semibold">
        {tableTitle}
      </Heading>
      <TableContainer>
        <Table variant="simple" colorScheme="blue" size="sm">
          <Thead>
            <Tr>
              <Th width="md">Value</Th>
              <Th width="xs">Modified</Th>
              <Th width="xs">Action</Th>
            </Tr>
          </Thead>
          <Tbody>{tableBody}</Tbody>
        </Table>
      </TableContainer>
    </VStack>
  );

  const RegRuleTableRow = ({ rule }) => {
    const EditRegistrationRuleState = useDisclosure();
    const DeleteRegistrationRuleState = useDisclosure();
    return (
      <Tr
        _hover={{
          bg: "gray.100",
        }}
      >
        <Td>{rule.value}</Td>
        <Td>{new Date(rule.modified).toLocaleString()}</Td>
        <Td>
          <HStack spacing={2}>
            <Button
              size="xs"
              colorScheme="blue"
              leftIcon={<FaLink />}
              onClick={EditRegistrationRuleState.onOpen}
            >
              Edit
            </Button>
            {EditRegistrationRuleState.isOpen && (
              <ModalEditRegistratonRule
                isModalOpen={EditRegistrationRuleState.isOpen}
                onModalClose={EditRegistrationRuleState.onClose}
                registrationRule={rule}
              />
            )}
            <Button
              size="xs"
              colorScheme="red"
              leftIcon={<FaTrash />}
              onClick={DeleteRegistrationRuleState.onOpen}
            >
              Delete
            </Button>
            {DeleteRegistrationRuleState.isOpen && (
              <ModalDeleteRegistrationRule
                isModalOpen={DeleteRegistrationRuleState.isOpen}
                onModalClose={DeleteRegistrationRuleState.onClose}
                registrationRule={rule}
              />
            )}
          </HStack>
        </Td>
      </Tr>
    );
  };

  return (
    <VStack align="stretch" px={8} w="100%" spacing={4} p={4}>
      <SortingAndFilteringPanel
        data={registrationRules}
        sorting={sorting}
        onSort={onSort}
        filter={filter}
        setFilter={setFilter}
        sortPanelLabel="Email/Email Domain"
        sortPanelKey={sortingStorageKey}
        NewButton={NewButton}
        emptyAlert="No rules found yet."
      />
      {NewRegistrationRuleState.isOpen && (
        <ModalNewRegistratonRule
          isModalOpen={NewRegistrationRuleState.isOpen}
          onModalClose={NewRegistrationRuleState.onClose}
        />
      )}

      {Object.keys(registrationRules).length > 0 && (
        <Center>
          <VStack spacing={16} pb={8}>
            <RegRulesTable
              as
              Block
              tableTitle="Block list"
              tableBody={outputList.map(
                (item) =>
                  item.isBlocked && (
                    <RegRuleTableRow key={item.id} rule={item} />
                  )
              )}
            />

            <RegRulesTable
              tableTitle="Allow list"
              tableBody={outputList.map(
                (item) =>
                  !item.isBlocked && (
                    <RegRuleTableRow key={item.id} rule={item} />
                  )
              )}
            />
          </VStack>
        </Center>
      )}
    </VStack>
  );
};
