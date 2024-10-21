import { HStack, Input, Alert, AlertIcon } from "@chakra-ui/react";
import { SortPanel } from "./SortPanel";

export const SortingAndFilteringPanel = ({
  data,
  sorting,
  onSort,
  filter,
  setFilter,
  sortPanelLabel,
  sortPanelKey,
  NewButton,
  emptyAlert,
}) => {
  return (
    <HStack justify="end" p={4}>
      {Object.keys(data).length ? (
        <>
          <SortPanel
            state={sorting}
            onSortButtonClick={onSort}
            keys={[[sortPanelLabel, sortPanelKey]]}
          />

          <Input
            flex={1}
            variant="flushed"
            placeholder={`Filter by ${sortPanelLabel}`}
            size="sm"
            value={filter}
            onChange={({ target: { value } }) => setFilter(value)}
          />
        </>
      ) : (
        <Alert status="info">
          <AlertIcon />
          {emptyAlert}
        </Alert>
      )}
      <NewButton />
    </HStack>
  );
};
