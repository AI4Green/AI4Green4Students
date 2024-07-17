import React from "react";
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  Box,
  Text,
} from "@chakra-ui/react";

export const Breadcrumbs = ({ items = [] }) => (
  <Breadcrumb
    separator=">"
    spacing="8px"
    padding="8px 16px"
    backgroundColor="gray.50"
    borderRadius="md"
    boxShadow="md"
  >
    {items.map((item, index) => (
      <BreadcrumbItem key={index} isCurrentPage={index === items.length - 1}>
        <BreadcrumbLink
          href={item.href}
          color={index === items.length - 1 ? "teal.500" : "blue.500"}
          fontWeight={index === items.length - 1 ? "bold" : "medium"}
          _hover={{ textDecoration: "underline", color: "blue.700" }}
        >
          <Text fontSize="lg">{item.label}</Text>
        </BreadcrumbLink>
      </BreadcrumbItem>
    ))}
  </Breadcrumb>
);
