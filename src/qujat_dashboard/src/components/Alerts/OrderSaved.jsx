import React, { useState, useEffect } from "react";
import { Stack, Alert, Slide, LinearProgress } from "@mui/material";

function OrderSaved() {
  const [showAlert, setShowAlert] = useState(true);

  useEffect(() => {
    const timer = setTimeout(() => {
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
            sx={{ position: "absolute", top: "12%", right: "1.2%" }}
          >
            <Alert
              variant="filled"
              severity="success"
              onClose={() => {
                setShowAlert(false);
              }}
            >
              Өзгерістер қабылданды!!!
            </Alert>
          </Stack>
        </Slide>
      )}
    </>
  );
}

export default OrderSaved;
