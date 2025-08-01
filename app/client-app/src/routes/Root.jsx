import { Routes, Route, useNavigate } from "react-router-dom";
import { useEffect } from "react";
import { DefaultLayout } from "layouts/DefaultLayout";
import { ProtectedRoutes } from "layouts/ProtectedRoutes";
import { useUser } from "contexts";
import { ContentPage } from "pages/ContentPage";
import { NotFound } from "pages/error";
import { Home } from "pages/Home";
import { UserHome } from "pages/UserHome";
import { Account } from "./Account";
import { Admin } from "./Admin";
import { Project } from "./Project";
import GreenMetrics from "pages/GreenMetrics";

const ConditionalHome = () => {
  const { user } = useUser();
  if (user) {
    return (
      <Routes>
        <Route path="/" element={<DefaultLayout />}>
          <Route index element={<UserHome />} />
        </Route>
      </Routes>
    );
  }
  return <Home />;
};

export const Root = () => {
  return (
    <Routes>
      <Route index element={<ConditionalHome />} />

      <Route path="/" element={<DefaultLayout />}>
        <Route path="metrics" element={<GreenMetrics />} />
        <Route
          path="greenchemistry"
          element={<ContentPage contentKey={"greenchemistry"} />}
        />
        <Route path="about" element={<ContentPage contentKey={"about"} />} />

        <Route
          path="documentation"
          element={<ContentPage contentKey={"documentation"} />}
        />

        <Route path="projects/*" element={<Project />} />

        <Route path="account/*" element={<Account />} />

        <Route path="admin/*" element={<Admin />} />

        <Route path="*" element={<NotFound />} />
      </Route>
    </Routes>
  );
};
