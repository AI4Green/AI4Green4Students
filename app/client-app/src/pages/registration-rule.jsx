import { HStack } from "@chakra-ui/react";
import { Breadcrumbs } from "components/core/breadcrumbs";
import { Table } from "components/registration-rule/table";
import { TITLE_ICON_COMPONENTS } from "constants";
import { DefaultContentHeader, DefaultContentLayout } from "layouts/default";

export const RegistrationRule = () => {
  const breadcrumbItems = [
    { label: "Home", href: "/" },
    {
      label: "Registration Rules",
    },
  ];

  return (
    <DefaultContentLayout>
      <Breadcrumbs items={breadcrumbItems} />
      <HStack>
        <DefaultContentHeader
          header="Registration Rules"
          icon={TITLE_ICON_COMPONENTS.RegistrationRule}
        />
      </HStack>
      <Table />
    </DefaultContentLayout>
  );
};
