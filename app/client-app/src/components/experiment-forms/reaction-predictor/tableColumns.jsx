import { Image, Flex } from "@chakra-ui/react";
import { DataTableColumnHeader } from "components/core/data-table";
import { Badge } from "@chakra-ui/react";

export const tableColumns = [
  {
    accessorKey: "product",
    header: () => (
      <DataTableColumnHeader sorting={false} title="Product SMILES" />
    ),
  },
  {
    accessorKey: "iupacName",
    header: () => <DataTableColumnHeader sorting={false} title="IUPAC Name" />,
  },
  {
    accessorKey: "score",
    header: () => <DataTableColumnHeader sorting={false} title="Score" />,
  },
  {
    accessorKey: "reactionImage",
    header: () => (
      <DataTableColumnHeader sorting={false} title="Reaction Image" />
    ),
    cell: ({ row }) => (
      <Image
        src={`data:image/png;base64,${row.original.reactionImage}`}
        alt="Reaction"
      />
    ),
  },
  {
    accessorKey: "Synonyms",
    header: () => <DataTableColumnHeader sorting={false} title="Synonyms" />,
    cell: ({ row }) => {
      const synonyms = row.original.synonyms;

      if (synonyms.length > 0) {
        return (
          <Flex flexWrap="wrap" gap={4} maxW="md">
            {synonyms.slice(0, 3).map((synonym) => (
              <Badge
                px={2}
                py={1}
                borderRadius={5}
                colorScheme="green"
                variant="outline"
                key={synonym}
              >
                {synonym}
              </Badge>
            ))}
          </Flex>
        );
      }
      return null;
    },
  },
];
