import { Badge, Flex, Image } from "@chakra-ui/react";
import { DataTableColumnHeader } from "components/core/data-table";

export const columns = [
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
        maxW="270px"
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
          <Flex flexWrap="wrap" gap={4} maxW="sm">
            {synonyms.slice(0, 3).map((synonym) => (
              <Badge
                px={2}
                py={0.5}
                borderRadius={5}
                colorScheme="green"
                variant="outline"
                key={synonym}
                fontSize="xxs"
                fontWeight="normal"
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
