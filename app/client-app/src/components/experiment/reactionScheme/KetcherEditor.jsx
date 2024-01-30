import { Text, VStack } from "@chakra-ui/react";
import { forwardRef } from "react";

export const KetcherEditor = forwardRef(function KetcherEditor({ ...p }, ref) {
  return (
    <VStack minW="full" align="flex-start">
      <Text as="b">Reaction Sketcher</Text>
      <iframe
        ref={ref}
        src="/js/ketcher/index.html"
        title="Ketcher App"
        {...p}
      />
    </VStack>
  );
});
