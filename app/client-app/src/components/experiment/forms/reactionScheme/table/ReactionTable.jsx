/**
 * The component renders a table.
 * Populates table using ketcher data if there's no existing table data.
 * Data is reset if ketcher user resets the ketcher.
 */

import {
  Button,
  HStack,
  Icon,
  IconButton,
  Popover,
  PopoverArrow,
  PopoverBody,
  PopoverCloseButton,
  PopoverContent,
  PopoverTrigger,
  Text,
  VStack,
  useDisclosure,
} from "@chakra-ui/react";
import { DataTable } from "components/dataTable/DataTable";
import { useCallback, useMemo } from "react";
import {
  FaCheckCircle,
  FaExclamationCircle,
  FaFlask,
  FaVial,
} from "react-icons/fa";
import { AddSubstanceModal } from "../modal/AddSubstanceModal";
import { reactionTableColumns } from "./reactionTableColumn";
import { useReactionTable } from "./useReactionTableData";

export const REACTION_TABLE_DEFAULT_VALUES = {
  tableData: [],
  massUnit: "",
};

export const ReactionTable = ({ name, ketcherData, isDisabled }) => {
  const { tableData, setTableData } = useReactionTable(name, ketcherData);

  const tableColumns = useMemo(
    () =>
      reactionTableColumns({
        isDisabled,
      }),
    [isDisabled]
  ); // array of objects to be used for the DataTable columns

  const footerCellProps = useCallback(() => ({ setTableData }), [setTableData]);

  return (
    <DataTable
      data={tableData}
      setTableData={setTableData}
      columns={tableColumns}
      FooterCellAddRow={!isDisabled && <FooterCell {...footerCellProps()} />}
    >
      <HStack flex={1}>
        <Text size="sm" as="b">
          Please fill in the relevant fields below
        </Text>
      </HStack>
    </DataTable>
  );
};

export const FooterCell = ({ setTableData }) => {
  const Reagent = useDisclosure();
  const Solvent = useDisclosure();

  return (
    <HStack justify="flex-end" spacing={5}>
      <Button
        colorScheme="pink"
        size="sm"
        leftIcon={<FaFlask />}
        onClick={Reagent.onOpen}
      >
        Add reagent
      </Button>
      <Button
        colorScheme="teal"
        size="sm"
        leftIcon={<FaVial />}
        onClick={Solvent.onOpen}
      >
        Add solvent
      </Button>
      <AddSubstanceModal
        isModalOpen={Reagent.isOpen}
        onModalClose={Reagent.onClose}
        setTableData={setTableData}
      />
      <AddSubstanceModal
        isModalOpen={Solvent.isOpen}
        onModalClose={Solvent.onClose}
        setTableData={setTableData}
        isAddingSolvent
      />
    </HStack>
  );
};

//TODO: add validation for the hazards against the backend
export const HazardsValidation = ({ input, valid }) => {
  if (!input || !valid) return null;

  return input === valid ? (
    <Icon as={FaCheckCircle} color="green.500" fontSize="lg" />
  ) : (
    <Popover>
      <PopoverTrigger>
        <IconButton
          aria-label="warning"
          icon={
            <Icon as={FaExclamationCircle} color="orange.500" fontSize="lg" />
          }
          size="sm"
          variant="ghost"
        />
      </PopoverTrigger>
      <PopoverContent color="white" bg="orange.500" aria-label="hazard-warning">
        <PopoverArrow />
        <PopoverCloseButton />
        <PopoverBody fontWeight="bold" border="0">
          Incorrect hazard code
        </PopoverBody>
      </PopoverContent>
    </Popover>
  );
};
