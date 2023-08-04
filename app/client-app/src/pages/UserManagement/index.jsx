import { Heading, VStack } from "@chakra-ui/react";
import { Userslist } from "components/userManagement/UsersList";
import { UserSummaryCard } from "components/userManagement/UserSummaryCard";
import { useUserList } from "api/user";

export const UserManagement = () => {
  const { data: usersList } = useUserList();
  return (
    <VStack align="stretch" px={8} w="100%" spacing={4} p={4}>
      <Heading as="h2" size="lg">
        List of all Users
      </Heading>
      <Userslist
        data={usersList}
        emptyAlert="No users found yet."
        sortingStorageKey="fullName"
      >
        {(summary) => {
          return <UserSummaryCard key={summary.id} user={summary} />;
        }}
      </Userslist>
    </VStack>
  );
};
