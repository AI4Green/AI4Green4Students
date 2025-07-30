import { ChakraProvider, Flex, Spinner } from "@chakra-ui/react";
import { StrictMode, Suspense } from "react";
import { createRoot } from "react-dom/client";
import { theme } from "./themes/theme";

import "config/i18n";
import { BrowserRouter } from "react-router-dom";
import { Root } from "./routes/Root";
import {
  UserProvider,
  BackendApiProvider,
  BackendConfigProvider,
} from "./contexts";
import { ErrorBoundary } from "./components/core/ErrorBoundary";

createRoot(document.getElementById("root")).render(
  <StrictMode>
    <ChakraProvider theme={theme}>
      <BrowserRouter>
        <ErrorBoundary>
          <Suspense
            fallback={
              <Flex justify="center" w="100%" my={16}>
                <Spinner boxSize={16} />
              </Flex>
            }
          >
            <BackendApiProvider>
              <UserProvider>
                <BackendConfigProvider>
                  <Root />
                </BackendConfigProvider>
              </UserProvider>
            </BackendApiProvider>
          </Suspense>
        </ErrorBoundary>
      </BrowserRouter>
    </ChakraProvider>
  </StrictMode>,
);
