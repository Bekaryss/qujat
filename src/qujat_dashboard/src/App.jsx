import React from "react";
import { useSelector, useDispatch } from "react-redux";
import Layout from "./components/Layout/Layout";
import Login from "./pages/Login/Login";
import { AuthMe } from "./redux/slices/adminSlice";
import { selectIsAuth } from "./redux/slices/tokenSlice";

import "./App.css";

const App = () => {
  //test
  const dispatch = useDispatch();
  const isAuth = useSelector(selectIsAuth);
  const [authChecked, setAuthChecked] = React.useState(false);

  React.useEffect(() => {
    const fetchData = async () => {
      try {
        await dispatch(AuthMe());
        setAuthChecked(true);
      } catch (error) {
        console.error("Error while checking authentication:", error);
        setAuthChecked(true);
      }
    };

    fetchData();
  }, [dispatch, isAuth]);

  if (!authChecked) {
    return null;
  }

  return <>{isAuth ? <Layout /> : <Login />}</>;
};

export default App;
