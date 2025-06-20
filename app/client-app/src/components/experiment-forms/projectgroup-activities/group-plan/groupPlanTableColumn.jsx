import {
  DataTableColumnHeader,
  TableCellDateInput,
  TableCellDeleteRowButton,
  TableCellTextAreaInput,
  WordCountBadge,
} from "components/core/data-table";
import {
  Accordion,
  AccordionButton,
  AccordionItem,
  AccordionPanel,
  VStack,
  Textarea,
  Text,
  AccordionIcon,
  Avatar,
  HStack,
} from "@chakra-ui/react";

export const groupPlanTableColumn = (students, userLoggedIn, isDisabled) => [
  {
    accessorKey: "serialNumber",
    header: ({ column }) => (
      <DataTableColumnHeader sorting={false} title="No." column={column} />
    ),
    enableHiding: false,
    meta: {
      width: 1,
      verticalAlign: "top",
    },
  },
  {
    accessorKey: "weekDate",
    header: () => <DataTableColumnHeader sorting={false} title="Week date" />,
    cell: ({ getValue, row, column, table }) => (
      <TableCellDateInput
        {...{ getValue, row, column, table }}
        placeholder="Week date"
        isDisabled={isDisabled}
      />
    ),
    meta: {
      width: 1,
      verticalAlign: "top",
    },
  },
  {
    accessorKey: "allPlan",
    header: () => <DataTableColumnHeader sorting={false} title="Group Plan" />,
    cell: ({ getValue, row, column, table }) => (
      <TableCellTextAreaInput
        {...{ getValue, row, column, table }}
        placeholder="Group Plan"
        isDisabled={isDisabled}
      />
    ),
    meta: {
      width: "sm",
      verticalAlign: "top",
    },
  },
  {
    id: "students",
    header: () => <DataTableColumnHeader sorting={false} title="Students" />,
    cell: ({ row, column, table }) => (
      <Accordion allowMultiple>
        {students.map((student) => (
          <Student
            key={student.id}
            isDisabled={userLoggedIn?.email !== student.email}
            student={student}
            row={row}
            column={column}
            table={table}
          />
        ))}
      </Accordion>
    ),
    meta: {
      verticalAlign: "top",
    },
  },

  ...(!isDisabled
    ? [
        {
          id: "actions",
          header: () => (
            <DataTableColumnHeader sorting={false} title="Actions" />
          ),
          cell: TableCellDeleteRowButton,
          meta: {
            width: 1,
          },
        },
      ]
    : []),
];

const Student = ({ student, row, table, isDisabled }) => {
  const getValue = () => row.original[student.id] || "";

  const setValue = (value) =>
    table.options.meta?.updateData(row.index, student.id, value);

  return (
    <AccordionItem>
      <AccordionButton
        _expanded={{ bg: "green.100" }}
        borderRadius={4}
        p={2}
        justifyContent="space-between"
      >
        <HStack spacing={2} fontSize="xs" fontWeight="medium">
          <Avatar size="xs" name={student.name} />
          <Text>{student.name}</Text>
        </HStack>

        <AccordionIcon />
      </AccordionButton>
      <AccordionPanel>
        <VStack align="flex-start">
          <Textarea
            rows="8"
            fontSize="sm"
            value={getValue()}
            onChange={(e) => setValue(e.target.value)}
            onBlur={() => setValue(getValue())}
            isDisabled={isDisabled}
            placeholder={`${student.name}'s plan`}
            _disabled={{ opacity: 0.7 }}
          />
          <WordCountBadge value={getValue()} />
        </VStack>
      </AccordionPanel>
    </AccordionItem>
  );
};
