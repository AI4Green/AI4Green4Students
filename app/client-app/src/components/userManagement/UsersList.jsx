import {
  Alert,
  AlertIcon,
  HStack,
  Input,
  VStack,
  Button,
  Text,
} from "@chakra-ui/react";
import { FaPlus } from "react-icons/fa";
import { SortPanel } from "components/SortPanel";
import { useSortingAndFiltering } from "helpers/hooks/useSortingAndFiltering";
import { ManageAction as NewInvite } from "./ManageAction";
import { actionMenu } from "./actionMenu";
import { useDisclosure } from "@chakra-ui/react";

const SortingAndFilteringPanel = ({
  data,
  sorting,
  onSort,
  filter,
  setFilter,
}) => {
  const NewInviteState = useDisclosure();
  const { inviteNew } = actionMenu(); // assign inviteNew action options
  return (
    <>
      <HStack justify="end" p={4}>
        {Object.keys(data).length && (
          <>
            <SortPanel
              state={sorting}
              onSortButtonClick={onSort}
              keys={[["Name", "fullName"]]}
            />

            <Input
              flex={1}
              variant="flushed"
              placeholder="Filter by Name"
              size="sm"
              value={filter}
              onChange={({ target: { value } }) => setFilter(value)}
            />
          </>
        )}
        <Button
          onClick={NewInviteState.onOpen}
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
      </HStack>
      {NewInviteState.isOpen && (
        <NewInvite
          isModalOpen={NewInviteState.isOpen}
          onModalClose={NewInviteState.onClose}
          actionSelected={inviteNew}
        />
      )}
    </>
  );
};

// Extending useSortingAndFiltering hook
const useUserSortingAndFiltering = (data, sortingStorageKey) =>
  useSortingAndFiltering(data, sortingStorageKey, {
    initialSort: { key: sortingStorageKey },
    sorters: {
      // sorters key name should match with 'sortingStorageKey'
      // for eg., sorters key is "fullName" below.
      // Thus, 'sortingStorageKey' should be "fullName" as well for this sort function to work
      fullName: {
        sorter: (asc) => (a, b) =>
          asc ? a.localeCompare(b) : b.localeCompare(a),
      },
    },
    storageKey: "users",
  });

export const Userslist = ({
  data,
  children,
  emptyAlert,
  sortingStorageKey,
}) => {
  const { sorting, onSort, filter, setFilter, outputList } =
    useUserSortingAndFiltering(data, sortingStorageKey);
  return Object.keys(data).length ? (
    <>
      <SortingAndFilteringPanel
        data={data}
        sorting={sorting}
        onSort={onSort}
        filter={filter}
        setFilter={setFilter}
      />
      <VStack align="stretch" px={4}>
        {outputList.map(children)}
      </VStack>
    </>
  ) : (
    <Alert status="info">
      <AlertIcon />
      {emptyAlert}
    </Alert>
  );
};
