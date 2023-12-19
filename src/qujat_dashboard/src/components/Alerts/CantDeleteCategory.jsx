import React, { useState, useEffect } from "react";
import { Stack, Alert, Slide, LinearProgress } from "@mui/material";
import { useDispatch } from "react-redux";
import { setError } from "../../redux/slices/categorySlice";

function CantDeleteCategory() {
  const dispatch = useDispatch();
  const [showAlert, setShowAlert] = useState(true);

  useEffect(() => {
    const timer = setTimeout(() => {
      dispatch(setError());
      setShowAlert(false);
    }, 3000); // Adjust the duration (in milliseconds) as needed

    return () => clearTimeout(timer);
  }, []); // Run the effect only once when the component mounts

  return (
    <>
      {showAlert && (
        <Slide direction="left" in={true} mountOnEnter unmountOnExit>
          <Stack
            direction="column"
            spacing={2}
            sx={{ position: "absolute", top: "15%", right: "1.2%" }}
          >
            <Alert
              variant="filled"
              severity="error"
              onClose={() => {
                setShowAlert(false);
              }}
            >
              Нельзя удалить категорию, пока существует хотя бы 1 подкатегория
              внутри!
            </Alert>
          </Stack>
        </Slide>
      )}
    </>
  );
}

export default CantDeleteCategory;
